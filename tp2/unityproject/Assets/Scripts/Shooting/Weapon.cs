using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Objects")]
    public Transform gunEndPoint;
    public ParticleSystem muzzleFlash;

    [Header("Bullets")]
    public bool emitBulletEvent = false;
    public int currentAmmo = 60;
    public int ammoPerMag = 60;
    public int extraAmmo = 360;
    public int totalInitialAmmo = 360;
    public int damage = 17;
    public float fireRate = .25f;
    public float range = 50f;
    public float hitForce = 100f;
    private float cooldownFire = 0f;
    private EvnBulletsChange evn;

    [Header("Animation")]
    public float animationDuration = 0.284f;
    public int animationIndex;
    public float animationHeadH;
    public float animationBodyH;
    
    [Header("Weapon")]
    public WeaponType type;
    public bool hasBullets = true;
    public bool canAim = false;
    public bool canRefill = true;

    [Header("Sounds")]

    public Sounds.AUDIO_TYPE reloadSound = Sounds.AUDIO_TYPE.GUN_PISTOL_RELOAD;
    public Sounds.AUDIO_TYPE shotSound = Sounds.AUDIO_TYPE.GUN_PISTOL_FIRE;

    [Header("Recoil")]

    public float recoilRangeTime = 0.5f;
    private float currentRecoilTime = 0f;
    public int cumulativeRecoilLimit = 5;
    private int cumulativeRecoil = 0;
    public float verticalRecoilStrength = 1f;
    public float horizontalRecoilStrength = 1f;

    void Start()
    {
        this.currentRecoilTime = this.recoilRangeTime;
        if (this.muzzleFlash) this.muzzleFlash.Stop();
        this.SendBulletsEvent();
    }

    void Update()
    {
        if (this.NeedsCooldown())
        {
            this.cooldownFire -= Time.deltaTime;
        } else {
            // Count towards recoil
            this.currentRecoilTime += Time.deltaTime;
            // Reduce recoil if time exceeded
            if (this.currentRecoilTime >= (this.recoilRangeTime / 2f) && this.cumulativeRecoil > 0) {
                this.cumulativeRecoil--;
                this.currentRecoilTime = 0f;
            }
        }
    }

    public bool NeedsReload()
    {
        return this.currentAmmo == 0;
    }

    public bool NeedsCooldown()
    {
        return this.cooldownFire > 0;
    }

    public bool CanFireShot()
    {
        return !this.hasBullets || (!(this.NeedsReload() || this.NeedsCooldown()));
    }

    public bool CanReload()
    {
        return (this.hasBullets && this.extraAmmo > 0 && this.ammoPerMag != this.currentAmmo);
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
        this.cooldownFire = this.fireRate;
        this.currentAmmo--;
        // Muzzle flash
        if (this.muzzleFlash) this.muzzleFlash.Play();
        // Send shot event
        this.SendBulletsEvent();
        // Determine if it has recoil
        if (this.currentRecoilTime <= this.recoilRangeTime) {
            this.cumulativeRecoil = Mathf.Min(this.cumulativeRecoilLimit, this.cumulativeRecoil + 1);
        }
        // Reset recoil time
        this.currentRecoilTime = 0f;
        return true;
    }

    public void SendBulletsEvent()
    {
        if (this.emitBulletEvent)
        {
            evn = EvnBulletsChange.notifier;
            evn.current = this.currentAmmo;
            evn.total = this.extraAmmo;
            evn.showBullets = this.hasBullets;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, evn);
        }
    }

    public void RefillWeapon()
    {
        if (this.canRefill)
        {
            this.currentAmmo = this.ammoPerMag;
            this.extraAmmo = this.totalInitialAmmo;
            this.SendBulletsEvent();
        }
    }

    public int HasRecoil() {
        return this.cumulativeRecoil;
    }

    public enum WeaponType
    {
        PISTOL, SHOTGUN, SNIPER, RIFLE, KNIFE
    }
}