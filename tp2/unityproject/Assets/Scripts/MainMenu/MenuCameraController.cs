using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    public float speed = 20f;
    public float moveDistance = 100f;

    private Vector3 moveVector = new Vector3(1, 0, 0);
    private float initialX;

    void Start()
    {
        this.initialX = this.transform.position.x;   
    }

    void Update()
    {
        this.transform.position = this.transform.position + this.moveVector * this.speed * Time.deltaTime;
        // Invert movement
        if (Mathf.Abs(this.transform.position.x - this.initialX) > this.moveDistance) {
            this.moveVector.x *= -1;
        }
    }
}
