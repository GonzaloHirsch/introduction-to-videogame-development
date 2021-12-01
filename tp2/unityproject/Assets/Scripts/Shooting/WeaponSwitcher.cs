using System.Collections;
using System.Collections.Generic;
using System;
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
        } else if (ActionMapper.GetWeapon3() && weaponSlots.Length >= 3) {
            this.weaponSwitchIndex = 3;
        } else if (ActionMapper.GetWeapon4() && weaponSlots.Length >= 4) {
            this.weaponSwitchIndex = 4;
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

    public void AddNewWeapon(GameObject weapon, int index) {
        // Generate slot
        WeaponSlot slot = new WeaponSlot();
        slot.weaponGo = weapon;
        slot.weaponIndex = index;
        // Append to slot list
        Array.Resize(ref this.weaponSlots, this.weaponSlots.Length + 1);
        this.weaponSlots[this.weaponSlots.Length - 1] = slot;
        // Add to map
        this.weapons[slot.weaponIndex] = slot.weaponGo.GetComponent<Weapon>();
    }

    public int GetNumberOfWeapons() {
        return this.weaponSlots.Length;
    }

    // Inner classes

    [System.Serializable]
    public class WeaponSlot {
        public GameObject weaponGo;
        public int weaponIndex;
    }
}
