using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IInteractable
{
    private Shooter shooter;
    private Thrower thrower;

    // Movement speed variables
    [Header("Movement")]
    public float speed;
    public float sprintSpeed;
    public float verticalSpeed;
    public float crouchSpeed;

    // Components
    private CharacterController cc;

    // Move variables
    private float horizontalMove;
    private float verticalMove;
    private float horizontalRotation;
    private float verticalRotation;
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool startedCrouching = false;
    private bool stoppedCrouching = false;
    private bool isCrouching = false;
    private bool jumped = false;
    private bool shoot = false;
    private bool reload = false;
    private bool aim = false;

    // Camera and rotation
    [Header("Camera and Rotation")]
    public float mouseSensitivity = 2f;
    public float upCameraLimit = -50f;
    public float downCameraLimit = 50f;
    private float currentVerticalRotation = 0f;
    private Camera fpsCam;                                                // Holds a reference to the first person camera

    // Aiming
    private bool isAiming = false;
    private EvnAim aimEvent;
    private float normalFov = 60f;
    public float aimFov = 5f;

    // Jumping
    private float currentJumpSpeed = 0f;
    private float currentYPosition = 0f;

    // Crouching
    private float initialHeight;

    [Header("Crouching")]
    [Range(0.0f, 1.0f)]
    public float crouchFactor = 0.5f;
    // The larger the faster we can crouch
    public float crouchTime = 5f;

    private Vector3 shootingDirection;

    void Start()
    {
        this.shooter = GetComponent<Shooter>();
        this.thrower = GetComponent<Thrower>();
        this.fpsCam = GetComponentInChildren<Camera>();
        // _shootableMask = LayerMask.GetMask("Shootable");

        this.cc = GetComponent<CharacterController>();
        this.initialHeight = this.cc.height;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Make sure to don't move if paused, some movements don't use timescale
        if (!GameStatus.Instance.GetGamePaused() && !this.shooter.isDead)
        {
            ReadInput();
            UpdateCameraRotation();
            if (!this.thrower.IsThrowingGrenade())
            {
                UpdateMovement();
                UpdateCrouching();
                CheckAiming();
                CheckShooting();
                CheckReloading();
            }
            else if (this.thrower.IsThrowingGrenade())
            {
                this.shooter.SetIdleAnimation();
                this.shooter.SetIdleSound();
            }
        }
    }

    void ReadInput()
    {
        this.horizontalMove = ActionMapper.GetMoveHorizontal();
        this.verticalMove = ActionMapper.GetMoveVertical();
        this.horizontalRotation = ActionMapper.GetCameraRotationHorizontal();
        this.verticalRotation = ActionMapper.GetCameraRotationVertical();
        this.jumped = ActionMapper.GetJump();
        this.isSprinting = ActionMapper.IsSprinting();
        this.startedCrouching = ActionMapper.StartedCrouching();
        this.stoppedCrouching = ActionMapper.StoppedCrouching();
        this.shoot = ActionMapper.GetShoot();
        this.aim = ActionMapper.GetAim();
        this.reload = ActionMapper.GetReload();
    }

    void UpdateMovement()
    {
        // Check it's not already jumping to avoid double jumping
        if (this.jumped && !this.isJumping && !this.isCrouching && this.cc.isGrounded)
        {
            this.currentJumpSpeed = this.verticalSpeed;
            this.isJumping = true;
        }

        // Check to determine jumping animation
        if (this.isJumping && this.jumped && this.cc.isGrounded)
        {
            this.shooter.SetStartJumpAnimation();
        }
        else
        {
            this.shooter.SetFinishJumpAnimation();
        }

        // Vf = V0 + g * dt
        float nextJumpSpeed = this.cc.isGrounded ? this.currentJumpSpeed : this.currentJumpSpeed + Physics.gravity.y * Time.deltaTime;
        // Dy = (Vi + Vf) * dt / 2
        float dy = (this.currentJumpSpeed + nextJumpSpeed) * Time.deltaTime / 2f;
        this.currentJumpSpeed = nextJumpSpeed;
        currentYPosition = (this.transform.position.y - this.cc.height) + dy;

        Vector3 gravityMove = new Vector3(0, dy, 0);
        Vector3 move = this.transform.forward * this.verticalMove + this.transform.right * this.horizontalMove;
        this.cc.Move((this.isCrouching ? this.crouchSpeed : (this.isSprinting ? this.sprintSpeed : this.speed)) * Time.deltaTime * move + gravityMove);

        if (currentYPosition < 0f)
        {
            currentYPosition = 0f;
            this.isJumping = false;
        }

        if (!Mathf.Approximately(this.verticalMove, 0f) || !Mathf.Approximately(this.horizontalMove, 0f))
        {
            if (this.isSprinting)
            {
                this.shooter.SetRunAnimation();
                this.shooter.SetRunSound();
            }
            else
            {
                this.shooter.SetWalkAnimation();
                this.shooter.SetWalkSound();
            }
        }
        else
        {
            this.shooter.SetIdleAnimation();
            this.shooter.SetIdleSound();
        }
    }

    void UpdateCameraRotation()
    {
        this.transform.Rotate(0, this.horizontalRotation * this.mouseSensitivity, 0);
        this.currentVerticalRotation += -this.verticalRotation * this.mouseSensitivity;
        this.currentVerticalRotation = Mathf.Clamp(this.currentVerticalRotation, this.upCameraLimit, this.downCameraLimit);
        this.shooter.SetBodyRotationAnimation(this.currentVerticalRotation);

    }

    void UpdateCrouching()
    {
        if (this.startedCrouching)
        {
            this.isCrouching = true;
            this.shooter.SetCrouchAnimation(true);
        }
        else if (this.stoppedCrouching)
        {
            this.isCrouching = false;
            this.shooter.SetCrouchAnimation(false);
        }
    }

    void CheckReloading()
    {
        // When reloading
        if (this.reload && this.shooter.CanReload())
        {
            this.shooter.Reload();
        }
    }

    void CheckShooting()
    {
        Transform camaraTransform = this.fpsCam.transform;
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane));
        this.shootingDirection = camaraTransform.forward;

        this.shooter.DebugDrawRay(rayOrigin, this.shootingDirection);

        // If shooting and not reloading
        if (this.shoot && this.shooter.CanShoot())
        {
            int recoil = this.shooter.HasRecoil();
            // Apply recoil if necessary
            if (recoil > 0) {
                this.shootingDirection = this.shooter.ApplyRecoil(this.shootingDirection, recoil);
            }
            // Shoot the weapon
            this.shooter.ShootWithMask(new Ray(camaraTransform.position, this.shootingDirection), LayerMask.GetMask("Enemy", "Default"));
        }
        // When shooting action is stopped
        else if (!this.shoot)
        {
            this.shooter.FinishShooting();
        }
    }

    void CheckAiming()
    {
        // If trying to aim and have the weapon can do it
        if (this.aim && this.shooter.CanAim() && !this.isAiming)
        {
            this.isAiming = true;
            // Set the aim event
            this.aimEvent = EvnAim.notifier;
            this.aimEvent.isAiming = true;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, this.aimEvent);
            this.fpsCam.fieldOfView = this.aimFov;
        }
        else if (!this.aim && this.isAiming)
        {
            this.isAiming = false;
            // Set the aim event
            this.aimEvent = EvnAim.notifier;
            this.aimEvent.isAiming = false;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, this.aimEvent);
            this.fpsCam.fieldOfView = this.normalFov;
        }
    }

    public Shooter GetShooter()
    {
        return this.shooter;
    }

    public void Interact()
    {
        // Do nothing since this method
        // is for Player interactions.
        // Will only use for enemy interactions.
    }

    public void InteractWithCaller(GameObject caller)
    {
        // Do nothing since this method
        // is for Player interactions.
        // Will only use for enemy interactions.
    }

    public InteractType GetInteractType()
    {
        return InteractType.Player;
    }

    void OnDrawGizmos()
    {
        Camera camera = GetComponentInChildren<Camera>();
        Gizmos.color = Color.red;

        Vector3 p1 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
        Vector3 p2 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.farClipPlane));

        Gizmos.DrawSphere(camera.transform.position, 0.1F);
        Gizmos.DrawLine(p1, p2);
    }
}
