using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Weapon weapon;
    private GameObject weaponGo;
    private Camera fpsCam;                                                // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine; 
    private bool isShooting = false;                                       // Reference to the LineRenderer component which will display our laserline
    private bool isReloading = false;  

    void Start() {
        this.weaponGo = Helper.FindChildGameObjectWithTag(this.gameObject, "Weapon");
        if (this.weaponGo != null) {
            this.weapon = this.weaponGo.GetComponent<Weapon>();
        }
        this.fpsCam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        this.laserLine = GetComponent<LineRenderer>();
        // _shootableMask = LayerMask.GetMask("Shootable");
    }
    
    void Update ()
    {
        // Create a vector at the center of our camera's viewport
        Vector3 lineOrigin = this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));

        // Draw a line in the Scene View  from the point lineOrigin 
        // in the direction of fpsCam.transform.forward * weaponRange, using the color green
        Debug.DrawRay(lineOrigin, this.fpsCam.transform.forward * this.weapon.range, Color.green);

        // If shooting and not reloading
        if (ActionMapper.GetShoot() && !this.isReloading && !this.weapon.NeedsCooldown()) {
            // Shoot the weapon
            this.Shoot();
            // Change shooting state
            if (!this.isShooting) {
                this.isShooting = !this.isShooting;
            }
        }
        // When shooting action is stopped
        else if (!ActionMapper.GetShoot())
        {
            if (this.isShooting) {
                this.isShooting = !this.isShooting;
            }
        }
        // When reloading
        if (ActionMapper.GetReload()) {
            this.StartReloading();
            this.FinishReloading();
        }
    }
     private void Shoot()
    {
        bool firedSuccess = this.weapon.ShotFired();
        Debug.Log("Fired successfully: " + firedSuccess);

        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        laserLine.SetPosition(0, this.weapon.gunEndPoint.position);
        
        StartCoroutine(ShotEffect());

        if (!firedSuccess) {
            // Handle sound for when weapon has no ammo
            // Will never fall here when cooldown is > 0 since it
            // is called from the update
        }

        // Check if the raycast collided with something
        if (Physics.Raycast(ray, out hit, this.weapon.range)) {
            print("hit " + hit.collider.gameObject);
            this.laserLine.SetPosition(1, hit.point);
            // Get the gameobject that collided with the raycast
            Shootable shootable = hit.collider.GetComponent<Shootable>();
            // If obj is shootable and a shot was fired, apply the weapon damage
            if (shootable != null && firedSuccess) {
                shootable.ApplyDamage(this.weapon.damage);
            }
        }
        else {
            // If no collision, draw until the end of the weapons range
            this.laserLine.SetPosition(1, ray.GetPoint(this.weapon.range));
        }
    }
    private void StartReloading()
    {
        Debug.Log("Reloading...");
        Debug.Log("Ammo before: " + this.weapon.currentAmmo);
        this.isShooting = false;
        this.isReloading = true;
    }
    // Should be called after animation is done
    private void FinishReloading() 
    {
        this.isReloading = false;
        this.weapon.Reload();
        Debug.Log("Ammo after: " + this.weapon.currentAmmo);
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
}
