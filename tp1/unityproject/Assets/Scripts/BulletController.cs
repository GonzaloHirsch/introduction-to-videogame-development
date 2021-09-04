using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string[] collisionTags = new string[] {Constants.TAG_ASTEROID};
    public float speed = 800f;
    public float timeToLive = 0.8f;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        initializeVelocityVector();
    }

    // Update is called once per frame
    void Update()
    {
        updatePosition();
        destroyIfExpired();
    }

    void destroyIfExpired() {
        this.timeToLive -= Time.deltaTime;
        if (timeToLive <= 0) {
            Destroy(this.gameObject);
        }
    }

    void updatePosition() {
        // Get delta time
        float dt = Time.deltaTime;
        // Multiply the time with the velocity to know the next position
        transform.position += this.velocity * dt;
    }

    void initializeVelocityVector() {
        this.velocity = transform.right * this.speed;
    }

    // Destroy
    void OnTriggerEnter2D(Collider2D other) {
        foreach(var tag in this.collisionTags) {
            if (other.gameObject.CompareTag(tag)) {
                Destroy(this.gameObject);
            }
        }
    }
}
