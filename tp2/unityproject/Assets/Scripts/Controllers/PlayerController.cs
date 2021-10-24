using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Shooter shooter;

    // Movement speed variables
    [Header("Movement")]
    public float speed;
    public float sprintSpeed;
    public float verticalSpeed;

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

    // Camera and rotation
    [Header("Camera and Rotation")]
    public float mouseSensitivity = 2f;
    public float upCameraLimit = -50f;
    public float downCameraLimit = 50f;
    private float currentVerticalRotation = 0f;
    private Camera fpsCam;                                                // Holds a reference to the first person camera

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

    void Start()
    {
        this.shooter = GetComponent<Shooter>();
        this.fpsCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        // _shootableMask = LayerMask.GetMask("Shootable");
    
        this.cc = GetComponent<CharacterController>();
        this.initialHeight = this.cc.height;
    }

    void Update()
    {
        // Make sure to don't move if paused, some movements don't use timescale
        if (!GameStatus.Instance.GetGamePaused() && !this.shooter.isDead)
        {
            ReadInput();
            UpdateMovement();
            UpdateCrouching();
            UpdateCameraRotation();
            CheckShooting();
            CheckReloading();
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
        this.cc.Move((this.isSprinting ? this.sprintSpeed : this.speed) * Time.deltaTime * move + gravityMove);

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
            }
            else
            {
                this.shooter.SetWalkAnimation();
            }
        }
        else
        {
            this.shooter.SetIdleAnimation();
        }
    }

    void UpdateCameraRotation()
    {
        this.transform.Rotate(0, this.horizontalRotation * this.mouseSensitivity, 0);
        // this.cameraTransform.Rotate(-this.verticalRotation*this.mouseSensitivity,0,0);
        // Debug.Log(-this.verticalRotation*this.mouseSensitivity);
        // Rotate hands accordingly
        // this.hands.transform.Rotate(-this.verticalRotation*this.mouseSensitivity, this.horizontalRotation * this.mouseSensitivity, 0);

        // Vector3 currentRotation = this.cameraTransform.localEulerAngles;
        // if (currentRotation.x > 180) currentRotation.x -= 360;
        // currentRotation.x = Mathf.Clamp(currentRotation.x, this.upCameraLimit, this.downCameraLimit);
        // this.cameraTransform.localRotation = Quaternion.Euler(currentRotation);

        this.currentVerticalRotation += -this.verticalRotation * this.mouseSensitivity;
        this.currentVerticalRotation = Mathf.Clamp(this.currentVerticalRotation, this.upCameraLimit, this.downCameraLimit);
        this.shooter.SetBodyRotationAnimation(this.currentVerticalRotation);

    }

    void UpdateCrouching()
    {
        /* float newHeight = this.initialHeight;

        if (this.isCrouching && !this.isJumping && !this.isSprinting) {
            newHeight = this.crouchFactor * this.initialHeight;
        }

        float lastHeight = this.cc.height;

        // lerp CharacterController height
        this.cc.height = Mathf.Lerp(this.cc.height, newHeight, this.crouchTime * Time.deltaTime );
        
        // fix vertical position
        this.transform.position = this.transform.position + new Vector3(0f, ( this.cc.height - lastHeight ) * this.crouchFactor, 0f);  */
        if (this.startedCrouching)
        {
            this.isCrouching = true;
            // this.cc.center = this.transform.forward + new Vector3(0f, this.cc.center.y, 0f);
            this.shooter.SetCrouchAnimation(true);
        }
        else if (this.stoppedCrouching)
        {
            this.isCrouching = false;
            // this.cc.center = new Vector3(0f, this.cc.center.y, 0f);
            this.shooter.SetCrouchAnimation(false);
        }
    }

    void CheckReloading()
    {
        // When reloading
        if (this.reload)
        {
            this.shooter.Reload();
        }
    }

    void CheckShooting()
    {
        Transform camaraTransform = this.fpsCam.transform;
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane));

        this.shooter.DebugDrawRay(rayOrigin, camaraTransform.forward);

        // If shooting and not reloading
        if (this.shoot && this.shooter.CanShoot()) 
        {
            // Trigger animation
            this.shooter.HandleShootAnimation();
            // Shoot the weapon
            this.shooter.Shoot(new Ray(camaraTransform.position, camaraTransform.forward));
        }
        // When shooting action is stopped
        else if (!this.shoot)
        {
            this.shooter.FinishShooting();
        }
    }

    public Shooter GetShooter()
    {
        return this.shooter;
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
