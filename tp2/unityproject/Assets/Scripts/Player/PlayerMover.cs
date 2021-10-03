using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    // Movement speed variables
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
    private bool isCrouching = false;
    private bool jumped = false;

    // Camera and rotation
    private Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float upCameraLimit = -50f;
    public float downCameraLimit = 50f;

    // Jumping
    private float currentJumpSpeed = 0f;
    private float currentYPosition = 0f;

    // Crouching
    private float initialHeight;
    
    [Range(0.0f, 1.0f)]
    public float crouchFactor = 0.5f;
    // The larger the faster we can crouch
    public float crouchTime = 5f;

    void Start()
    {
        this.cc = GetComponent<CharacterController>();
        this.initialHeight = this.cc.height;
        this.cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        ReadInput();
        UpdateMovement();
        UpdateCrouching();
        UpdateCameraRotation();
    }

    void ReadInput() {
        this.horizontalMove = ActionMapper.GetMoveHorizontal();
        this.verticalMove = ActionMapper.GetMoveVertical();
        this.horizontalRotation = ActionMapper.GetCameraRotationHorizontal();
        this.verticalRotation = ActionMapper.GetCameraRotationVertical();
        this.jumped = ActionMapper.GetJump();
        this.isSprinting = ActionMapper.IsSprinting();
        this.isCrouching = ActionMapper.IsCrouching();
    }

    void UpdateMovement() {
        // Check it's not already jumping to avoid double jumping
        if (this.jumped && !this.isJumping && !this.isCrouching) {
            this.currentJumpSpeed = this.verticalSpeed;
            this.isJumping = true;
        }

        // Vf = V0 + g * dt
        float nextJumpSpeed = this.currentJumpSpeed + Physics.gravity.y * Time.deltaTime;
        // Dy = (Vi + Vf) * dt / 2
        float dy = (this.currentJumpSpeed + nextJumpSpeed) * Time.deltaTime / 2f;
        this.currentJumpSpeed = nextJumpSpeed;
        currentYPosition = (this.transform.position.y - this.cc.height) + dy;

        Vector3 gravityMove = new Vector3(0, dy, 0);
        Vector3 move = this.transform.forward * this.verticalMove + this.transform.right * this.horizontalMove;
        this.cc.Move((this.isSprinting ? this.sprintSpeed : this.speed) * Time.deltaTime * move + gravityMove);

        if (currentYPosition < 0f) {
            currentYPosition = 0;
            this.isJumping = false;
            this.transform.position = new Vector3(this.transform.position.x, this.cc.height, transform.position.z);
        }
    }

    void UpdateCameraRotation()
    {
        this.transform.Rotate(0, this.horizontalRotation * this.mouseSensitivity, 0);
        this.cameraTransform.Rotate(-this.verticalRotation*this.mouseSensitivity,0,0);

        Vector3 currentRotation = this.cameraTransform.localEulerAngles;
        if (currentRotation.x > 180) currentRotation.x -= 360;
        currentRotation.x = Mathf.Clamp(currentRotation.x, this.upCameraLimit, this.downCameraLimit);
        this.cameraTransform.localRotation = Quaternion.Euler(currentRotation);
    }

    void UpdateCrouching(){
        float newHeight = this.initialHeight;

        // Debug.Log(this.isCrouching);
        // Debug.Log(this.isCrouching && !this.isJumping && !this.isSprinting);
        if (this.isCrouching && !this.isJumping && !this.isSprinting) {
            newHeight = this.crouchFactor * this.initialHeight;
        }

        float lastHeight = this.cc.height;

        // lerp CharacterController height
        this.cc.height = Mathf.Lerp(this.cc.height, newHeight, this.crouchTime * Time.deltaTime );
        
        // fix vertical position
        this.transform.position = this.transform.position + new Vector3(0f, ( this.cc.height - lastHeight ) * this.crouchFactor, 0f); 
    }
}
