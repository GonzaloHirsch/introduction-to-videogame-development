using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrower : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject grenadePrefab;

    [Header("Throw Action")]
    public float force = 1000f;
    public float cooldown = 10f;
    private float timeBetweenThrows = 0f;
    public int intialAmmo = 3;
    private int ammo = 3;
    public float grenadeOriginDistance = 5f;
    public float throwAnimationTime = 1.6f;
    public float completeAnimationTime = 2.3f;

    [Header("Internal Variables")]
    private Camera fpsCam;
    private Vector3 grenadeOrigin;
    private GameObject thrownGrenade;
    private Rigidbody thrownGrenadeRb;
    private Grenade thrownGrenadeScript;
    private EvnGrenadesChange evn;
    public bool isDead = false;

    private bool isHolding = false;

    private bool isThrowing = false;

    private Animator characterAnimator;

    void Awake() {
        this.fpsCam = GetComponentInChildren<Camera>();
        this.characterAnimator = GetComponent<Animator>();
        // Make sure it can throw without waiting at the start
        this.timeBetweenThrows = this.cooldown;
        this.ammo = this.intialAmmo;
    }

    void Start()
    {
        this.SendThrowEvent();
    }

    void Update()
    {
        if (!this.isDead)
        {
            this.timeBetweenThrows += Time.deltaTime;
            this.CheckIfThrow();
        }
    }

    void CheckIfThrow()
    {
        // Make sure it has ammo and the cooldown is ok
        if (!this.isHolding && ActionMapper.GetGrenade() && this.ammo > 0 && this.timeBetweenThrows >= this.cooldown){
            this.isHolding = true;
            this.ActivateGrenade();
        } else if (this.isHolding && !ActionMapper.GetGrenade()) {
            this.isHolding = false;
            this.ThrowGrenade();
        }
    }

    void ActivateGrenade()
    {
        // Update internal state
        this.ammo--;
        // Generate grenade
        this.grenadeOrigin = (this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane))) + (this.fpsCam.transform.forward * this.grenadeOriginDistance);
        this.thrownGrenade = Instantiate(this.grenadePrefab, grenadeOrigin, Quaternion.identity);
        this.thrownGrenadeRb = this.thrownGrenade.GetComponent<Rigidbody>();
        this.thrownGrenadeScript = this.thrownGrenade.GetComponent<Grenade>();
        // Mark as kinematic to keep with the character
        this.thrownGrenadeRb.isKinematic = true;
        // Set grenade as active
        this.thrownGrenadeScript.SetGrenadeLive();
        // Sending the throw event
        this.SendThrowEvent();
    }
    
    void ThrowGrenade()
    {
        // Make it visible
        this.SetThrowAnimation();
    }

    void SendThrowEvent()
    {
        evn = EvnGrenadesChange.notifier;
        evn.current = this.ammo;
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, evn);
    }

    public void RefillGrenades() {
        this.ammo = this.intialAmmo;
        this.SendThrowEvent();
    }

    public void SetThrowAnimation()
    {
        this.isThrowing = true;
        // Store the previous weapon
        int prevWeapon = this.characterAnimator.GetInteger("WeaponType_int");
        // Mark as grenade
        this.characterAnimator.SetInteger("WeaponType_int", 10);    // Grenade
        // Use coroutine to make it look better, store the previous direction to not be affected by animation
        StartCoroutine(this.reallyThrowGrenade(this.throwAnimationTime, this.fpsCam.transform.forward));
        // Revert after 2
        StartCoroutine(this.revertAnimation(this.completeAnimationTime, prevWeapon));
    }

    IEnumerator revertAnimation(float secs, int animation)
    {
        yield return new WaitForSeconds(secs);
        
        this.characterAnimator.SetInteger("WeaponType_int", animation);
    }
    
    IEnumerator reallyThrowGrenade(float secs, Vector3 storedDirection)
    {
        yield return new WaitForSeconds(secs);

        // Update internal state
        this.timeBetweenThrows = 0f;
        this.isThrowing = false;
        // Add force as a throw
        this.thrownGrenadeRb.isKinematic = false;
        this.thrownGrenadeRb.AddForce(storedDirection * this.force);
        this.thrownGrenadeScript.ThrowGrenade();
    }

    public bool IsThrowingGrenade() {
        return this.isThrowing || this.isHolding;
    }
}
