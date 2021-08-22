using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float velocity = 10f;
    public int rotation;

    // PRIVATE VARIABLES
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        // Find player to determine position
        // TODO: VER QUE ESTO NO FALLE SI NO HAY PLAYER
        this.player = GameObject.FindGameObjectsWithTag("Player")[0];
        // Initially rotate random degrees
        this.rotation = Random.Range(0, 360);
        transform.eulerAngles = Vector3.forward * this.rotation;
        // Initial random position outside player area
        transform.position = this.GetRandomInitialPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // Get delta time
        float dt = Time.deltaTime;
        // Move in the direction it's facing using the transform.right vector
        transform.position += transform.right * dt * this.velocity;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("TIGGER ENTER");
    }

    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log("OnCollisionEnter2D");
    }

    // CUSTOM FUNCTIONS

    Vector3 GetRandomInitialPosition() {
        bool isPositionOk = false;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector3 position = Vector3.zero;
        while (!isPositionOk) {
            position = new Vector3(Random.Range(-screenWidth/2, screenWidth/2), Random.Range(-screenHeight/2, screenHeight/2), 0f);
            isPositionOk = Vector3.Distance(position, this.player.transform.position) >= Constants.MIN_DISTANCE_FROM_PLAYER;
        }
        return position;
    }
}
