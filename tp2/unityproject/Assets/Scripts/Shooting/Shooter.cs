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

    void Start() {
        this.weaponGo = Helper.FindChildGameObjectWithTag(this.gameObject, "Weapon");
        if (this.weaponGo != null) {
            this.weapon = this.weaponGo.GetComponent<Weapon>();
        }
    }
     private void Shoot()
    {
        Ray ray = fpsCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();

        laserLine.SetPosition(0, this.weapon.gunEndPoint.position);
        // StartCoroutine(FireLine());

        // Check if the raycast collided with something
        if (Physics.Raycast(ray, out hit, this.weapon.range)) {
            print("hit " + hit.collider.gameObject);
            this.laserLine.SetPosition(1, hit.point);
            // Get the gameobject that collided with the raycast
            Shootable shootable = hit.collider.GetComponent<Shootable>();
            // If GO is shootable, apply the weapon damage
            if (shootable != null) {
                shootable.ApplyDamage(this.weapon.damage);
            }
        }
        else
        {
            // If no collision, draw until the end of the weapons range
            this.laserLine.SetPosition(1, ray.GetPoint(this.weapon.range));
        }
    }
}
