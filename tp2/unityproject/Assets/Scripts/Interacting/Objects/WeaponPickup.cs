using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public string weaponLabel;
    private bool isAlreadyUsed = false;
    public GameObject weaponGo;

    public void Interact()
    {
        // Avoid calls to interact on this one, we expect to receive the caller
    }

    public void InteractWithCaller(GameObject caller)
    {
        if (!this.isAlreadyUsed) {
            // Mark as used
            this.isAlreadyUsed = true;
            // Add weapon
            this.PickupWeapon(caller);
            // Destroy
            Destroy(this.gameObject);
        }
    }

    public InteractType GetInteractType()
    {
        return InteractType.Weapon;
    }

    public string GetWeaponLabel() {
        return this.weaponLabel;
    }

    private void PickupWeapon(GameObject caller){
        // Get switcher from caller
        WeaponSwitcher switcher = caller.GetComponentInChildren<WeaponSwitcher>();
        int index = switcher.GetNumberOfWeapons() + 1;
        switcher.AddNewWeapon(this.weaponGo, index);
        // Send UI message
        EvnMessage evn = EvnMessage.notifier;
        evn.message = "Added " + this.weaponLabel + " at slot " + index + "!";
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, evn);
    }
}
