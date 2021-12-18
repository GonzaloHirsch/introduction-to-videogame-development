using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float maxDamage = 200f;
    public float explosionRadius = 10f;
    public float baseDamageMultiplier = 0.5f;
    public float timeToExplode = 5f;
    private float currentTime = 0f;
    private bool isActive = false;
    private bool exploded = false;

    public GameObject explosionEffectPrefab;
    private MeshRenderer meshRenderer;
    private AudioSource audioSource;

    void Awake()
    {
        this.meshRenderer = GetComponentInChildren<MeshRenderer>();
        this.audioSource = GetComponent<AudioSource>();
        if (this.audioSource != null) {
            this.audioSource.maxDistance = this.explosionRadius * 5f;
        }
    }

    void Start()
    {
        this.meshRenderer.enabled = false;
    }

    void Update()
    {
        if (this.isActive && !this.exploded)
        {
            this.currentTime += Time.deltaTime;
            this.CheckIfExplode();
        }
    }

    void CheckIfExplode()
    {
        if (this.currentTime >= this.timeToExplode)
        {
            this.exploded = true;
            this.Explode();
        }
    }

    void Explode()
    {
        // Show explosion
        if (this.explosionEffectPrefab != null) GameObject.Instantiate(this.explosionEffectPrefab, this.transform.position, Quaternion.identity);
        // Play sound
        if (this.audioSource != null) this.audioSource.Play();
        // Look for all objects to damage
        LayerMask mask = LayerMask.GetMask("Enemy", "Player");
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius, mask);
        bool hitPlayer = false;
        foreach (Collider nearbyObject in colliders)
        {
            if (!(nearbyObject.gameObject.CompareTag("Player") && hitPlayer)) {
                Shootable obj = nearbyObject.GetComponent<Shootable>();
                if (obj != null)
                {
                    obj.ApplyDamage(this.GetDamage(Vector3.Distance(this.transform.position, obj.transform.position)));
                }
                hitPlayer = hitPlayer || nearbyObject.gameObject.CompareTag("Player");
            }
        }
        if (this.audioSource != null) Destroy(this.gameObject, this.audioSource.clip.length / 2f);
        else Destroy(this.gameObject);
    }

    float GetDamageMultiplier(float distance)
    {
        return 1 + (this.baseDamageMultiplier - 1) * Mathf.Sqrt(distance / this.explosionRadius);
    }

    int GetDamage(float distance)
    {
        return (int)(this.GetDamageMultiplier(distance) * this.maxDamage);
    }

    public void SetGrenadeLive()
    {
        this.isActive = true;
    }

    public void ThrowGrenade() {
        this.meshRenderer.enabled = true;
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, this.explosionRadius);
    }
}
