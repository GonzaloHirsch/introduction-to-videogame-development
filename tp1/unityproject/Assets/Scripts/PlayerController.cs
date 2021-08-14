using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float velocity = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.right);
        Debug.Log(transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        var dt = Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) {
            transform.position = new Vector3(transform.position.x + dt * this.velocity, transform.position.y, transform.position.z);
        }
    }
}
