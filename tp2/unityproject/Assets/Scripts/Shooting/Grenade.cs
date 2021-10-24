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

    public GameObject explosionEffectPrefab;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        this.meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        Debug.Log(this.meshRenderer);
        this.meshRenderer.enabled = false;
    }

    void Update()
    {
        if (this.isActive)
        {
            this.currentTime += Time.deltaTime;
            this.CheckIfExplode();
        }
    }

    void CheckIfExplode()
    {
        if (this.currentTime >= this.timeToExplode)
        {
            this.Explode();
        }
    }

    void Explode()
    {
        // Show explosion
        if (this.explosionEffectPrefab != null) GameObject.Instantiate(this.explosionEffectPrefab, this.transform.position, Quaternion.identity);
        // Look for all objects to damage
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, this.explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Shootable obj = nearbyObject.GetComponent<Shootable>();
            if (obj != null)
            {
                obj.ApplyDamage(this.GetDamage(Vector3.Distance(this.transform.position, obj.transform.position)));
            }
        }
        Destroy(this.gameObject);
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
