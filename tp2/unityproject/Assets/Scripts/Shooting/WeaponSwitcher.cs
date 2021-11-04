using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public WeaponSlot[] weaponSlots;
    public Dictionary<int, Weapon> weapons = new Dictionary<int, Weapon>();
    public int initialWeaponIndex = 0;

    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;
    private EvnWeaponChange evn;

    [Header("Player")]
    public Shooter shooter;

    [Header("Input")]
    private int weaponSwitchIndex = -1;

    void Awake() {
        // Get all weapons
        foreach (WeaponSlot slot in this.weaponSlots) {
            this.weapons[slot.weaponIndex] = slot.weaponGo.GetComponent<Weapon>();
            if (slot.weaponIndex == this.initialWeaponIndex) {
                this.currentWeapon = this.weapons[slot.weaponIndex];
            }
        }
        // Set indexes
        this.currentWeaponIndex = this.initialWeaponIndex;
    }

    void Update()
    {
        this.GetSwitchInput();
        this.HandleSwitchWeapon();
    }

    // Actions

    void GetSwitchInput() {
        if (ActionMapper.GetWeapon1()) {
            this.weaponSwitchIndex = 1;
        } else if (ActionMapper.GetWeapon2()) {
            this.weaponSwitchIndex = 2;
        } else {
            this.weaponSwitchIndex = -1;
        }
    }

    void HandleSwitchWeapon() {
        // Is switching
        if (this.weaponSwitchIndex > 0 && this.weaponSwitchIndex != this.currentWeaponIndex) {
            // Switch index
            this.currentWeaponIndex = this.weaponSwitchIndex;
            // Store the new weapon
            this.currentWeapon = this.weapons[this.currentWeaponIndex];
            // Update weapons visibility
            this.ManageWeaponsState();
            // Update player weapon
            this.shooter.SetWeapon(this.currentWeapon);
            // Notify the UI
            this.currentWeapon.SendBulletsEvent();
            // Send weapon change to UI
            evn = EvnWeaponChange.notifier;
            evn.weaponType = this.weapons[this.currentWeaponIndex].type;
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, this.evn);
        }
    }

    void ManageWeaponsState() {
        for (int i = 0; i < this.weaponSlots.Length; i++){
            this.weaponSlots[i].weaponGo.SetActive(this.weaponSlots[i].weaponIndex == this.currentWeaponIndex);
        }
    }

    // Public mehtods

    public Weapon GetCurrentWeapon() {
        return this.currentWeapon;
    }

    // Inner classes

    [System.Serializable]
    public class WeaponSlot {
        public GameObject weaponGo;
        public int weaponIndex;
    }
}
