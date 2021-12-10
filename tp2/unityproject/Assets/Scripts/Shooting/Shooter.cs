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
    private WaitForSeconds reloadDuration = new WaitForSeconds(1.5f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;
    private bool isShooting = false;                                       // Reference to the LineRenderer component which will display our laserline
    private bool isReloading = false;
    public bool isDebug = true;
    public bool isDead = false;

    [Header("Sounds")]
    public bool playGlobalSound = false;
    private AudioManager audioManager;

    void Start()
    {
        this.weapon = this.GetComponentInChildren<Weapon>();
        this.laserLine = GetComponent<LineRenderer>();
        this.characterAnimator = GetComponent<Animator>();
        this.shotDuration = new WaitForSeconds(this.weapon.animationDuration);

        // Set initial animation, start idle
        this.SetIdleAnimation();
        this.SetWeaponAnimation();

        // _shootableMask = LayerMask.GetMask("Shootable");
        this.audioManager = GetComponent<AudioManager>();
    }

    //*****************************************//
    //*************SHOOTING METHODS************//
    //*****************************************//

    private void ApplyCollisionDamage(RaycastHit hit)
    {
        Shootable shootable = null;
        int damage = this.weapon.damage;

        if (hit.collider.CompareTag("EnemyHead")) {
            shootable = hit.collider.GetComponentInParent<Shootable>();
            damage = (int)Mathf.Ceil(shootable.maxHealth);
        } else {
            shootable = hit.collider.GetComponent<Shootable>();
        } 

        if (shootable != null) {
            shootable.ApplyDamage(damage);
        }
    }

    public bool CanShoot()
    {
        return !this.isReloading && !this.weapon.NeedsCooldown();
    }

    public void Shoot(Ray ray)
    {
        // Trigger animation
        this.HandleShootAnimation();
        // Shoot the weapon
        this.ShootWithRaycast(ray, LayerMask.GetMask("EnemyHead", "Enemy", "Player", "Default"));
    }

    public void ShootWithMask(Ray ray, int layerMask)
    {
        // Trigger animation
        this.HandleShootAnimation();
        // Shoot the weapon
        this.ShootWithRaycast(ray, layerMask);
    }

    public bool ShootWithRaycast(Ray ray, int layerMask)
    {
        RaycastHit hit = new RaycastHit();

        if (this.isDebug) this.laserLine.SetPosition(0, this.weapon.gunEndPoint.position);

        bool shotFired = this.weapon.ShotFired();
        // Check if the raycast collided with something
        if (Physics.Raycast(ray, out hit, this.weapon.range, layerMask))
        {
            if (this.isDebug) this.laserLine.SetPosition(1, hit.point);

            // Apply damage to the obj if a shot was fired
            if (shotFired)
            {
                this.ApplyCollisionDamage(hit);
                // Instantiate bullet hole if not player or enemy hit
                // No hole if with knife
                if (!hit.collider.CompareTag("Enemy") && !hit.collider.CompareTag("Player") && this.weapon.hasBullets)
                {
                    BulletHolePool.SharedInstance.ActivatePooledObject(hit.point + (hit.normal * 0.025f), Quaternion.FromToRotation(Vector3.forward, hit.normal));
                }
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
            this.SetEmptyGunSound();
            // Automatic reload when the player clicks
            this.AutomaticReload();
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

    public bool CanReload()
    {
        return this.weapon.CanReload();
    }

    public void Reload()
    {
        this.StartReloading();
        StartCoroutine(this.ReloadEffect());
    }

    private void StartReloading()
    {
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
    }

    private IEnumerator ReloadEffect()
    {
        //Wait for reload to finish
        yield return reloadDuration;
        // After waiting, set reload logic and finish animation
        this.FinishReloading();
    }

    private void AutomaticReload()
    {
        // Reload automatically if no ammo in current mag
        if (this.weapon.NeedsReload() && this.CanReload())
        {
            this.Reload();
        }

    }

    //*****************************************//
    //**************AIM METHODS**************//
    //*****************************************//

    public bool CanAim()
    {
        return this.weapon.canAim;
    }

    //*****************************************//
    //*************RECOIL METHODS************//
    //*****************************************//

    public int HasRecoil()
    {
        return this.weapon.HasRecoil();
    }

    public Vector3 ApplyRecoil(Vector3 originalDirection, int recoilNumber)
    {
        // Calculate X and Y drift
        float _recoilX = ((float)recoilNumber) / (2f * this.weapon.cumulativeRecoilLimit);
        float recoilX = this.GetRecoilDelta(_recoilX, this.weapon.horizontalRecoilStrength);
        float recoilY = this.GetRecoilDelta(_recoilX, this.weapon.verticalRecoilStrength);
        // Only apply in Y and X
        originalDirection.x += (Mathf.Sign(Random.Range(-1f, 1f)) * recoilX);
        originalDirection.y += recoilY;
        return originalDirection;
    }

    private float GetRecoilDelta(float x, float m)
    {
        return Mathf.Exp(m * x) - 1;
    }

    //*****************************************//
    //**************EXTRA METHODS**************//
    //*****************************************//

    public void SetDead(bool status)
    {
        this.isDead = status;
        // Mark as dead not to be able to shoot
        if (this.isDead)
        {
            Shooter s = this.GetComponent<Shooter>();
            if (s != null)
            {
                s.isDead = true;
            }
            Thrower t = this.GetComponent<Thrower>();
            if (t != null)
            {
                t.isDead = true;
            }
        }
    }

    public void DebugDrawRay(Vector3 rayOrigin, Vector3 rayDirection)
    {
        // Draw a line in the Scene View  from the point lineOrigin 
        // in the direction of fpsCam.transform.forward * weaponRange, using the color green
        if (this.isDebug)
        {
            Debug.DrawRay(rayOrigin, rayDirection * this.weapon.range, Color.green);
        }
    }

    public void SetWeapon(Weapon _weapon)
    {
        this.weapon = _weapon;
        this.SetWeaponAnimation();
    }

    //*****************************************//
    //************ANIMATION METHODS************//
    //*****************************************//

    public void SetWeaponAnimation()
    {
        this.characterAnimator.SetInteger("WeaponType_int", this.weapon.animationIndex);
        this.characterAnimator.SetFloat("Head_Horizontal_f", this.weapon.animationHeadH);
        this.characterAnimator.SetFloat("Body_Horizontal_f", this.weapon.animationBodyH);
    }

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
        if (isReloading)
        {
            this.SetReloadSound();
        }
    }
    public void SetShootAnimation(bool isShooting)
    {
        this.characterAnimator.SetBool("Shoot_b", isShooting);
        if (isShooting)
        {
            this.SetShotSound();
        }
    }

    //*****************************************//
    //************SOUND METHODS************//
    //*****************************************//

    public void SetRunSound()
    {
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_WALK);
            AudioManagerSingleton.Instance.Play(Sounds.AUDIO_TYPE.ENTITY_RUN, true);
        }
    }

    public void SetWalkSound()
    {
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Play(Sounds.AUDIO_TYPE.ENTITY_WALK, true);
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_RUN);
        }
        else
        {
            this.audioManager.Play(Sounds.AUDIO_TYPE.ENTITY_WALK, true);
        }
    }

    public void SetIdleSound()
    {
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_WALK);
            AudioManagerSingleton.Instance.Stop(Sounds.AUDIO_TYPE.ENTITY_RUN);
        }
        else
        {
            this.audioManager.Stop(Sounds.AUDIO_TYPE.ENTITY_WALK);
        }
    }

    public void SetShotSound()
    {
        if (this.playGlobalSound)
        {
            // Can overlap, multiple guns at the same time
            AudioManagerSingleton.Instance.Play(this.weapon.shotSound);
        }
        else
        {
            this.audioManager.Play(Sounds.AUDIO_TYPE.GUN_PISTOL_FIRE);
        }
    }

    public void SetReloadSound()
    {
        if (this.playGlobalSound)
        {
            StartCoroutine(this.playReloadSoundCoroutine(1.1f));
        }
    }

    IEnumerator playReloadSoundCoroutine(float secs)
    {
        yield return new WaitForSeconds(secs);
        if (this.playGlobalSound)
        {
            AudioManagerSingleton.Instance.Play(this.weapon.reloadSound);
        }
        else
        {
            this.audioManager.Play(Sounds.AUDIO_TYPE.GUN_PISTOL_RELOAD);

        }
    }

    public void SetEmptyGunSound()
    {
        if (this.playGlobalSound)
        {
            // Can overlap, multiple guns at the same time
            AudioManagerSingleton.Instance.Play(Sounds.AUDIO_TYPE.GUN_PISTOL_EMPTY);
        }
    }
}
