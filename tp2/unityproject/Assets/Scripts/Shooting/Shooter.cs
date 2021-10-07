using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Weapon weapon;
    private GameObject weaponGo;
    private Camera fpsCam;                                                // Holds a reference to the first person camera
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);    // WaitForSeconds object used by our ShotEffect coroutine, determines time laser line will remain visible
    private LineRenderer laserLine;                                        // Reference to the LineRenderer component which will display our laserline
    private float nextFire;                                                // Float to store the time the player will be allowed to fire again, after firing
    
    void Start() {
        this.weaponGo = Helper.FindChildGameObjectWithTag(this.gameObject, "Weapon");
        if (this.weaponGo != null) {
            this.weapon = this.weaponGo.GetComponent<Weapon>();
        }
    }
    void Shoot(Vector3 origin, Vector3 direction) {

    }
}
