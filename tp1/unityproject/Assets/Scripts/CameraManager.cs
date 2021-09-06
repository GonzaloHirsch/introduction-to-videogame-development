using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera cam;

    void Awake() {
        this.cam = GetComponent<Camera>();
    }

    void Start()
    {
        this.cam.orthographicSize = Mathf.RoundToInt(Screen.height / 2);   
    }
}
