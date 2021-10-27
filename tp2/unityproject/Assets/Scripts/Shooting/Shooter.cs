using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private Animator characterAnimator;

    [Header("Weapon")]
    public Weapon weapon;
    private GameObject weaponGo;
    private WaitForSeconds shotDuration;    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private WaitForSeconds reloadDuration = new WaitForSeconds(1.4f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;
    private bool isShooting = false;                                       // Reference to the LineRenderer component which will display our laserline
    private bool isReloading = false;
    public bool isDebug = true;
    public bool isDead = false;

    void Start()
    {
        this.weapon = this.GetComponentInChildren<Weapon>();
        this.laserLine = GetComponent<LineRenderer>();
        this.characterAnimator = GetComponent<Animator>();
        this.shotDuration = new WaitForSeconds(this.weapon.fireRate);

        // Set initial animation, start idle
        this.SetIdleAnimation();

        // _shootableMask = LayerMask.GetMask("Shootable");
    }

    //*****************************************//
    //*************SHOOTING METHODS************//
    //*****************************************//

    private void ApplyCollisionDamage(RaycastHit hit)
    {
        Shootable shootable = hit.collider.GetComponent<Shootable>();
        if (shootable != null)
        {
            // If obj is shootable, apply the weapon damage
            shootable.ApplyDamage(this.weapon.damage);
        }
    }

    public bool CanShoot()
    {
        return !this.isReloading && !this.weapon.NeedsCooldown();
    }

    public bool Shoot(Ray ray)
    {
        RaycastHit hit = new RaycastHit();

        if (this.isDebug) this.laserLine.SetPosition(0, this.weapon.gunEndPoint.position);

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
            if (this.isDebug) this.laserLine.SetPosition(1, ray.GetPoint(this.weapon.range));
        }
        // Returns if the shot was fired
        return shotFired;
    }

    public void HandleShootAnimation()
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

    public void FinishShooting()
    {
        // Setting class variable state
        this.isShooting = false;
        // Triggering the animation
        this.SetShootAnimation(this.isShooting);
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

    private IEnumerator ShotEffect()
    {
        // Turn on our line renderer
        if (this.isDebug) this.laserLine.enabled = true;

        yield return this.shotDuration;

        this.FinishShooting();
        // Deactivate our line renderer after waiting
        if (this.isDebug) this.laserLine.enabled = false;
    }

    //*****************************************//
    //************RELOADING METHODS************//
    //*****************************************//

    public void Reload()
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

    //*****************************************//
    //**************EXTRA METHODS**************//
    //*****************************************//

    public void SetDead(bool status) {
        this.isDead = status;
        // Mark as dead not to be able to shoot
        if (this.isDead) {
            Shooter s = this.GetComponent<Shooter>();
            if (s != null) {
                s.isDead = true;
            }
            Thrower t = this.GetComponent<Thrower>();
            if (t != null) {
                t.isDead = true;
            }
        }
    }
    
    public void DebugDrawRay(Vector3 rayOrigin, Vector3 rayDirection)
    {
        // Draw a line in the Scene View  from the point lineOrigin 
        // in the direction of fpsCam.transform.forward * weaponRange, using the color green
        if (this.isDebug) {
            Debug.DrawRay(rayOrigin, rayDirection * this.weapon.range, Color.green);
        }
    }

    //*****************************************//
    //************ANIMATION METHODS************//
    //*****************************************//

    public void SetIdleAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 0f);
    }

    public void SetWalkAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 0.5f);
    }

    public void SetRunAnimation()
    {
        this.characterAnimator.SetFloat("Speed_f", 1f);
    }

    public void SetBodyRotationAnimation(float angleDeg)
    {
        this.characterAnimator.SetFloat("Body_Vertical_f", angleDeg * Mathf.PI / 180 * -1);
    }

    public void SetStartJumpAnimation()
    {
        this.characterAnimator.SetBool("Jump_b", true);
        this.characterAnimator.SetBool("Grounded", false);
    }

    public void SetFinishJumpAnimation()
    {
        this.characterAnimator.SetBool("Jump_b", false);
        this.characterAnimator.SetBool("Grounded", true);
    }

    public void SetCrouchAnimation(bool status)
    {
        this.characterAnimator.SetBool("Crouch_b", status);
    }
    public void SetReloadAnimation(bool isReloading)
    {
        this.characterAnimator.SetBool("Reload_b", isReloading);
    }
    public void SetShootAnimation(bool isShooting)
    {
        this.characterAnimator.SetBool("Shoot_b", isShooting);
    }
}
