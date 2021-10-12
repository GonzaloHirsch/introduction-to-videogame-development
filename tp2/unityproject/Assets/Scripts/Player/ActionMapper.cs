using UnityEngine;

public class ActionMapper
{
    public static float GetMoveHorizontal() {
        return Input.GetAxis("Horizontal");
    }

    public static float GetMoveVertical() {
        return Input.GetAxis("Vertical");
    }

    public static float GetCameraRotationHorizontal() {
        return Input.GetAxis("Mouse X");
    }
    
    public static float GetCameraRotationVertical() {
        return Input.GetAxis("Mouse Y");
    }

    public static bool GetShoot() {
        return Input.GetMouseButtonDown(0);
    }

    public static bool GetReload() {
        return Input.GetKeyDown(KeyCode.R);
    }

    public static bool GetJump() {
        return Input.GetKeyDown(KeyCode.Space);
    }
    
    public static bool IsSprinting() {
        return Input.GetKey(KeyCode.LeftShift);
    }

    public static bool IsInteracting() {
        return Input.GetKey(KeyCode.E);
    }
    
    public static bool StartedCrouching() {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }

    public static bool StoppedCrouching() {
        return Input.GetKeyUp(KeyCode.LeftControl);
    }
}