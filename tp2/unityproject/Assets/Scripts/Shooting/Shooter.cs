using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private Animator characterAnimator;
    public Weapon weapon;
    private GameObject weaponGo;
    private Camera fpsCam;                                                // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private WaitForSeconds reloadDuration = new WaitForSeconds(1.4f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;
    private bool isShooting = false;                                       // Reference to the LineRenderer component which will display our laserline
    private bool isReloading = false;
    public bool isDebug = false;

    void Start()
    {
        this.weaponGo = Helper.FindChildGameObjectWithTag(this.gameObject, "Weapon");
        if (this.weaponGo != null)
        {
            this.weapon = this.weaponGo.GetComponent<Weapon>();
        }
        this.fpsCam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        this.laserLine = GetComponent<LineRenderer>();
        this.characterAnimator = GetComponent<Animator>();
        // _shootableMask = LayerMask.GetMask("Shootable");
    }

    void Update()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 rayOrigin = this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane));

        // Draw a line in the Scene View  from the point lineOrigin 
        // in the direction of fpsCam.transform.forward * weaponRange, using the color green
        if (this.isDebug) Debug.DrawRay(rayOrigin, this.fpsCam.transform.forward * this.weapon.range, Color.green);

        // If shooting and not reloading
        if (ActionMapper.GetShoot() && !this.isReloading && !this.weapon.NeedsCooldown())
        {
            // Trigger animation
            this.HandleShootAnimation();
            // Shoot the weapon
            bool shotFired = this.Shoot();
        }
        // When shooting action is stopped
        else if (!ActionMapper.GetShoot())
        {
            this.FinishShooting();
        }

        // When reloading
        if (ActionMapper.GetReload())
        {
            this.Reload();
        }

    }

    private bool Shoot()
    {
        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        if (this.isDebug) laserLine.SetPosition(0, this.weapon.gunEndPoint.position);

        bool shotFired = this.weapon.ShotFired();

        // Check if the raycast collided with something
        if (Physics.Raycast(ray, out hit, this.weapon.range))
        {

            print("hit " + hit.collider.gameObject);

            if (this.isDebug) this.laserLine.SetPosition(1, hit.point);

            // Apply damage to the obj if a shot was fired
            if (shotFired)
            {
                this.ApplyCollisionDamage(hit);
            }
        }
        else if (this.isDebug)
        {
            // If no collision, draw until the end of the weapons range
            this.laserLine.SetPosition(1, ray.GetPoint(this.weapon.range));
        }
        // Returns if the shot was fired
        return shotFired;
    }

    private void ApplyCollisionDamage(RaycastHit hit)
    {
        Shootable shootable = hit.collider.GetComponent<Shootable>();
        if (shootable != null)
        {
            // If obj is shootable, apply the weapon damage
            shootable.ApplyDamage(this.weapon.damage);
        }
    }

    private void HandleShootAnimation()
    {
        bool canFire = this.weapon.CanFireShot();
        Debug.Log("Fired successfully: " + canFire);

        if (canFire)
        {
            // Start the shooting animation
            this.StartShooting();
        }
        else
        {
            /**
             * Handle sound/animation for when weapon has no ammo
             * Will never fall here when cooldown is > 0 since it
             * is called from the update
             */
        }
    }

    private void StartShooting()
    {
        Debug.Log("Shooting...");
        // Setting laser effect
        StartCoroutine(this.ShotEffect());
        // Setting class variable state
        this.isShooting = true;
        // Triggering the animation
        this.SetShootAnimation(this.isShooting);
    }

    private void FinishShooting()
    {
        // Setting class variable state
        this.isShooting = false;
        // Triggering the animation
        this.SetShootAnimation(this.isShooting);
    }

    private IEnumerator ShotEffect()
    {
        // Turn on our line renderer
        this.laserLine.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        this.laserLine.enabled = false;
    }

    private void Reload()
    {
        this.StartReloading();
        StartCoroutine(this.ReloadEffect());
    }

    private void StartReloading()
    {
        Debug.Log("Reloading...");
        Debug.Log("Ammo before: " + this.weapon.currentAmmo);
        // Setting class variable state
        this.isShooting = false;
        this.isReloading = true;
        // Triggering the animation
        this.SetShootAnimation(this.isShooting);
        this.SetReloadAnimation(this.isReloading);
    }

    private void FinishReloading()
    {
        this.isReloading = false;
        // Triggering the animation
        this.SetReloadAnimation(this.isReloading);
        // Add the reloaded bullets to the weapon ammo
        this.weapon.Reload();
        // Debug
        Debug.Log("Ammo after: " + this.weapon.currentAmmo);
    }

    private IEnumerator ReloadEffect()
    {
        //Wait for reload to finish
        yield return reloadDuration;
        // After waiting, set reload logic and finish animation
        this.FinishReloading();
    }

    void SetReloadAnimation(bool isReloading)
    {
        this.characterAnimator.SetBool("Reload_b", isReloading);
    }
    void SetShootAnimation(bool isShooting)
    {
        this.characterAnimator.SetBool("Shoot_b", isShooting);
    }
}
