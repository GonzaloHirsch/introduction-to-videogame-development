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
    private bool jumped = false;

    // camera and rotation
    private Transform cameraTransform;
    public float mouseSensitivity = 2f;
    public float upCameraLimit = -50f;
    public float downCameraLimit = 50f;

    private float currentJumpSpeed = 0f;
    private float currentYPosition = 0f;

    void Start()
    {
        this.cc = GetComponent<CharacterController>();
        this.cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        ReadInput();
        UpdateMovement();
        UpdateCameraRotation();
    }

    void ReadInput() {
        this.horizontalMove = ActionMapper.GetMoveHorizontal();
        this.verticalMove = ActionMapper.GetMoveVertical();
        this.horizontalRotation = ActionMapper.GetCameraRotationHorizontal();
        this.verticalRotation = ActionMapper.GetCameraRotationVertical();
        this.jumped = ActionMapper.GetJump();
        this.isSprinting = ActionMapper.IsSprinting();
    }

    void UpdateMovement() {
        // Check it's not already jumping to avoid double jumping
        if (this.jumped && !this.isJumping) {
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

    public void UpdateCameraRotation()
    {
        this.transform.Rotate(0, this.horizontalRotation * this.mouseSensitivity, 0);
        this.cameraTransform.Rotate(-this.verticalRotation*this.mouseSensitivity,0,0);

        Vector3 currentRotation = this.cameraTransform.localEulerAngles;
        if (currentRotation.x > 180) currentRotation.x -= 360;
        currentRotation.x = Mathf.Clamp(currentRotation.x, this.upCameraLimit, this.downCameraLimit);
        this.cameraTransform.localRotation = Quaternion.Euler(currentRotation);
    }
}
