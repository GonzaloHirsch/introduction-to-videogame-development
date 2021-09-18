using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public string[] collisionTags = new string[] {Constants.TAG_ASTEROID};
    public float speed = 800f;
    public float timeToLive = 0.8f;
    private float currentTime;
    private Vector3 velocity;
    
    void OnEnable()
    {
        Debug.Log("HERE");
        initializeVelocityVector();
        restartTimeToLive();
        this.playShootingSound();
    }

    void Update()
    {
        updatePosition();
        destroyIfExpired();
    }

    void restartTimeToLive() {
        this.currentTime = this.timeToLive;
    }

    void destroyIfExpired() {
        this.currentTime -= Time.deltaTime;
        if (currentTime <= 0) {
            this.gameObject.SetActive(false);
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
                this.gameObject.SetActive(false);
            }
        }
    }

    // Sound
    void playShootingSound() {
        AudioManager.Instance.Play(Constants.AUDIO_TYPE.BULLET_FIRE);
    }
}
