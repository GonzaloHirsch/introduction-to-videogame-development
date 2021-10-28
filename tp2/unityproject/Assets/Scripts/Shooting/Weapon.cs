using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform gunEndPoint;
    public ParticleSystem muzzleFlash;
    public bool emitBulletEvent = false;
    public int currentAmmo = 60;
    public int ammoPerMag = 60;
    public int extraAmmo = 360;
    public int totalInitialAmmo = 360;
    public int damage = 17;
    public float fireRate = .25f;
    public float animationDuration = 0.284f;
    public float range = 50f;
    public float hitForce = 100f;
    private float cooldownFire = 0f;
    private EvnBulletsChange evn;

    void Start(){
        if (this.muzzleFlash) this.muzzleFlash.Stop();
        this.SendBulletsEvent();
    }

    void Update()
    {
        if (this.NeedsCooldown())
        {
            this.cooldownFire -= Time.deltaTime;
        }
    }

    bool NeedsReload()
    {
        return this.currentAmmo == 0;
    }

    public bool NeedsCooldown()
    {
        return this.cooldownFire > 0;
    }

    public bool CanFireShot()
    {
        return !(this.NeedsReload() || this.NeedsCooldown());
    }

    public bool CanReload(){
        return this.extraAmmo > 0 && this.ammoPerMag != this.currentAmmo;
    }

    public void Reload()
    {
        // Do not reload if the mag is full
        if (this.ammoPerMag == this.currentAmmo)
        {
            return;
        }
        int ammoToRefill = Mathf.Min(this.ammoPerMag - this.currentAmmo, this.extraAmmo);
        this.currentAmmo += ammoToRefill;
        this.extraAmmo -= ammoToRefill;
        // Send the change event
        this.SendBulletsEvent();
    }

    public bool ShotFired()
    {
        if (!this.CanFireShot())
        {
            return false;
        }
        Debug.Log("Fired: " + this.currentAmmo);
        this.cooldownFire = this.fireRate;
        this.currentAmmo--;
        // Muzzle flash
        if (this.muzzleFlash) this.muzzleFlash.Play();
        // Send shot event
        this.SendBulletsEvent();
        return true;
    }

    void SendBulletsEvent()
    {
        if (this.emitBulletEvent) {
            evn = EvnBulletsChange.notifier;
            evn.current = this.currentAmmo;
            evn.total = this.extraAmmo;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, evn);
        }
    }

    public void RefillWeapon() {
        this.currentAmmo = this.ammoPerMag;
        this.extraAmmo = this.totalInitialAmmo;
        this.SendBulletsEvent();
    }
}