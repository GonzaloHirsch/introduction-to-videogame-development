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
    private EvnGrenadesChange evn;

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
        this.timeBetweenThrows += Time.deltaTime;
        this.CheckIfThrow();
    }

    void CheckIfThrow()
    {
        // Make sure it has ammo and the cooldown is ok
        if (ActionMapper.GetGrenade() && this.ammo > 0 && this.timeBetweenThrows >= this.cooldown)
        {
            this.ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        // Update internal state
        this.ammo--;
        this.timeBetweenThrows = 0f;
        // Generate grenade
        this.grenadeOrigin = (this.fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, this.fpsCam.nearClipPlane))) + (this.fpsCam.transform.forward * this.grenadeOriginDistance);
        this.thrownGrenade = Instantiate(this.grenadePrefab, grenadeOrigin, Quaternion.identity);
        // Add force as a throw
        this.thrownGrenadeRb = this.thrownGrenade.GetComponent<Rigidbody>();
        this.thrownGrenadeRb.AddForce(this.fpsCam.transform.forward * this.force);
        // Sending the throw event
        this.SendThrowEvent();
    }

    void SendThrowEvent() {
        evn = EvnGrenadesChange.notifier;
        evn.current = this.ammo;
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, evn);
    }
}
