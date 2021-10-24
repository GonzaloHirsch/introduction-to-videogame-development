using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private Camera fpsCam;                                                // Holds a reference to the first person camera
    private Shooter shooter;
    public bool isDead = false;

    void Start()
    {
        this.shooter = GetComponent<Shooter>();
        Debug.Log("Start: " + this.shooter);
        this.fpsCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        // _shootableMask = LayerMask.GetMask("Shootable");
    }

    void Update()
    {
        if (!this.isDead)
        {
            // Create a vector at the center of our camera's viewport
            Vector3 rayOrigin = this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane));

            this.shooter.DebugDrawRay(rayOrigin, this.fpsCam.transform.forward);

            // If shooting and not reloading
            if (ActionMapper.GetShoot() && this.shooter.CanShoot()) 
            {
                // Trigger animation
                this.shooter.HandleShootAnimation();
                // Shoot the weapon
                this.Shoot();
            }
            // When shooting action is stopped
            else if (!ActionMapper.GetShoot())
            {
                this.shooter.FinishShooting();
            }

            // When reloading
            if (ActionMapper.GetReload())
            {
                this.shooter.Reload();
            }
        }
    }

    private bool Shoot()
    {
        Ray ray = new Ray(this.fpsCam.transform.position, this.fpsCam.transform.forward);
        return this.shooter.Shoot(ray);
    }

    void OnDrawGizmos()
    {
        Camera camera = GetComponentInChildren<Camera>();
        Gizmos.color = Color.red;

        Vector3 p1 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
        Vector3 p2 = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.farClipPlane));

        Gizmos.DrawSphere(camera.transform.position, 0.1F);
        Gizmos.DrawLine(p1, p2);
    }
}
