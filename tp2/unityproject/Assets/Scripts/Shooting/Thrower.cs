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

    [Header("Internal Variables")]
    private Camera fpsCam;
    private Vector3 grenadeOrigin;
    private GameObject thrownGrenade;
    private Rigidbody thrownGrenadeRb;
    private Grenade thrownGrenadeScript;
    private EvnGrenadesChange evn;
    public bool isDead = false;

    private bool isHolding = false;

    void Start()
    {
        this.fpsCam = GetComponentInChildren<Camera>();
        // Make sure it can throw without waiting at the start
        this.timeBetweenThrows = this.cooldown;
        this.ammo = this.intialAmmo;
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
        if (ActionMapper.GetGrenadeHold() && this.ammo > 0 && this.timeBetweenThrows >= this.cooldown)
        {
            this.isHolding = true;
            this.ActivateGrenade();
        } else if (this.isHolding && ActionMapper.GetGrenadeThrow()) {
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
        // Update internal state
        this.timeBetweenThrows = 0f;
        // Add force as a throw
        this.thrownGrenadeRb.isKinematic = false;
        this.thrownGrenadeRb.AddForce(this.fpsCam.transform.forward * this.force);
        // Make it visible
        this.thrownGrenadeScript.ThrowGrenade();
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
}
