using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Movement speed variables
    [Header("Movement")]
    public float speed;
    public float sprintSpeed;
    public float verticalSpeed;

    // Components
    private CharacterController cc;
    private Animator characterAnimator;

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
    private bool isDead = false;

    // Camera and rotation
    private Transform cameraTransform;
    [Header("Camera and Rotation")]
    public float mouseSensitivity = 2f;
    public float upCameraLimit = -50f;
    public float downCameraLimit = 50f;
    private float currentVerticalRotation = 0f;

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

    // Shooting
    [Header("Shooting")]
    private Transform gunEnd;
    private GameObject gun;

    void Start()
    {
        this.characterAnimator = GetComponent<Animator>();
        this.cc = GetComponent<CharacterController>();
        this.initialHeight = this.cc.height;
        this.cameraTransform = GetComponentInChildren<Camera>().transform;

        // Set initial animation, start idle
        this.SetIdleAnimation();
    }

    void Update()
    {
        // Make sure to don't move if paused, some movements don't use timescale
        if (!GameStatus.Instance.GetGamePaused() && !this.isDead)
        {
            ReadInput();
            UpdateMovement();
            UpdateCrouching();
            UpdateCameraRotation();
            CheckShooting();
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
    }

    void UpdateMovement()
    {
        // Check it's not already jumping to avoid double jumping
        if (this.jumped && !this.isJumping && !this.isCrouching)
        {
            this.currentJumpSpeed = this.verticalSpeed;
            this.isJumping = true;
        }

        // Check to determine jumping animation
        if (this.isJumping && this.jumped)
        {
            this.SetStartJumpAnimation();
        }
        else
        {
            this.SetFinishJumpAnimation();
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
            currentYPosition = 0;
            this.isJumping = false;
        }

        if (!Mathf.Approximately(this.verticalMove, 0f) || !Mathf.Approximately(this.horizontalMove, 0f))
        {
            if (this.isSprinting)
            {
                this.SetRunAnimation();
            }
            else
            {
                this.SetWalkAnimation();
            }
        }
        else
        {
            this.SetIdleAnimation();
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
        this.SetBodyRotationAnimation(this.currentVerticalRotation);

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
            this.SetCrouchAnimation(true);
        }
        else if (this.stoppedCrouching)
        {
            this.isCrouching = false;
            // this.cc.center = new Vector3(0f, this.cc.center.y, 0f);
            this.SetCrouchAnimation(false);
        }
    }

    void CheckShooting()
    {
        if (this.shoot)
        {
            // CAST RAY
        }
    }

    public void SetDead(bool status) {
        this.isDead = status;
        // Mark as dead not to be able to shoot
        if (this.isDead) {
            Shooter s = this.GetComponent<Shooter>();
            if (s != null) {
                s.isDead = true;
            }
            Thrower t = this.GetComponent<Thrower>();
            if (t != null) {
                t.isDead = true;
            }
        }
    }

    // Animator functions

    void SetIdleAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 0f);
    }

    void SetWalkAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 0.5f);
    }

    void SetRunAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 1f);
    }

    void SetBodyRotationAnimation(float angleDeg)
    {
        this.characterAnimator.SetFloat("Body_Vertical_f", angleDeg * Mathf.PI / 180 * -1);
    }

    void SetStartJumpAnimation()
    {
        this.characterAnimator.SetBool("Jump_b", true);
        this.characterAnimator.SetBool("Grounded", false);
    }

    void SetFinishJumpAnimation()
    {
        this.characterAnimator.SetBool("Jump_b", false);
        this.characterAnimator.SetBool("Grounded", true);
    }

    void SetCrouchAnimation(bool status)
    {
        this.characterAnimator.SetBool("Crouch_b", status);
    }
}
