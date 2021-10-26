using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    private bool isAlreadyUsed = false;
    private Outline outline;

    void Awake() {
        this.outline = GetComponent<Outline>();
    }

    public void Interact()
    {
        // Avoid calls to interact on this one, we expect to receive the caller
    }

    public void InteractWithCaller(GameObject caller)
    {
        if (!this.isAlreadyUsed) {
            this.GiveAmmo(caller);
        }
    }

    public InteractType GetInteractType()
    {
        return InteractType.AmmoBox;
    }

    private void GiveAmmo(GameObject caller){
        this.isAlreadyUsed = true;
        // Refill weapon
        Weapon weapon = caller.GetComponentInChildren<Weapon>();
        weapon.RefillWeapon();
        // Refill grenades
        Thrower thrower = caller.GetComponent<Thrower>();
        thrower.RefillGrenades();
        // Update outline
        this.outline.OutlineWidth = 0f;
    }

    public bool IsEmpty() {
        return this.isAlreadyUsed;
    }
}
