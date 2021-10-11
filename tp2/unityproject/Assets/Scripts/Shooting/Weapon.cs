using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform gunEndPoint;
    public int currentAmmo = 60;                                            
    public int ammoPerMag = 60;                                            
    public int extraAmmo = 360;                                            
    public int totalInitialAmmo = 360;                                            
    public int damage = 17;                                            
    public float fireRate = .25f;                                        
    public float range = 50f;                                        
    public float hitForce = 100f;
    private float cooldownFire = 0f;    
    
    void Update() {
        if (this.NeedsCooldown()) {
            this.cooldownFire -= Time.deltaTime;
        }
    }   

    bool NeedsReload() {
        return this.currentAmmo == 0;
    }

    public bool NeedsCooldown() {
        return this.cooldownFire > 0;
    }

    public bool CanFireShot() {
        return !(this.NeedsReload() || this.NeedsCooldown());
    }

    public void Reload() {
        // Do not reload if the mag is full
        if (this.ammoPerMag == this.currentAmmo) {
            return;
        }
        int ammoToRefill = Mathf.Min(this.ammoPerMag - this.currentAmmo, this.extraAmmo);
        this.currentAmmo += ammoToRefill;
        this.extraAmmo -= ammoToRefill;
    }

    public bool ShotFired() {
        if (!this.CanFireShot()) {
            return false;
        }
        Debug.Log("Fired: " + this.currentAmmo);
        this.cooldownFire = this.fireRate;
        this.currentAmmo--;
        return true;
    }
}