using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour, IInteractable
{
    private bool isAlreadyUsed = false;

    public void Interact()
    {
        if (!this.isAlreadyUsed) {
            this.GiveAmmo();
        }
    }

    public InteractType GetInteractType()
    {
        return InteractType.AmmoBox;
    }

    private void GiveAmmo(){
        this.isAlreadyUsed = true;
        // GIVE AMMO
    }

    public bool IsEmpty() {
        return this.isAlreadyUsed;
    }
}
