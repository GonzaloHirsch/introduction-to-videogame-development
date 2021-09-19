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
        this.InitializeVelocityVector();
        this.RestartTimeToLive();
        this.PlayShootingSound();
    }

    void Update()
    {
        this.UpdatePosition();
        this.DestroyIfExpired();
    }

    void RestartTimeToLive() {
        this.currentTime = this.timeToLive;
    }

    void DestroyIfExpired() {
        this.currentTime -= Time.deltaTime;
        if (currentTime <= 0) {
            this.gameObject.SetActive(false);
        }
    }

    void UpdatePosition() {
        // Get delta time
        float dt = Time.deltaTime;
        // Multiply the time with the velocity to know the next position
        transform.position += this.velocity * dt;
    }

    void InitializeVelocityVector() {
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
    void PlayShootingSound() {
        AudioManager.Instance.Play(Constants.AUDIO_TYPE.BULLET_FIRE);
    }
}
