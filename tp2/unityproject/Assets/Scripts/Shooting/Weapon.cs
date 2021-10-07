using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int currentAmmo = 60;                                            
    public int ammoPerMag = 60;                                            
    public int extraAmmo = 360;                                            
    public int totalInitialAmmo = 360;                                            
    public int gunDamage = 1;                                            
    public float fireRate = .25f;                                        
    public float weaponRange = 50f;                                        
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

    bool NeedsCooldown() {
        return this.cooldownFire > 0;
    }

    void Reload() {
        // Do not reload if the mag is full
        if (this.ammoPerMag == this.currentAmmo) {
            return;
        }
        int ammoToRefill = Mathf.Min(this.ammoPerMag - this.currentAmmo, this.extraAmmo);
        this.currentAmmo += ammoToRefill;
        this.extraAmmo -= ammoToRefill;
    }

    bool ShotFired() {
        if (this.NeedsReload() || this.NeedsCooldown()) {
            return false;
        }
        this.cooldownFire = this.fireRate;
        this.currentAmmo--;
        return true;
    }
}