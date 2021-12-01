using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{
    public GunIcon[] gunIcons;
    public Text currentBullets;
    public Text totalBullets;
    public Text currentGrenades;
    private EvnBulletsChange evnBullets;
    private EvnGrenadesChange evnGrenades;
    private EvnWeaponChange evnWeapon;

    void Awake()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBulletsChange.EventName, OnBulletsChange);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnGrenadesChange.EventName, OnGrenadesChange);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnWeaponChange.EventName, OnWeaponChange);
    }

    void OnBulletsChange(System.Object sender, FrameLord.GameEvent e)
    {
        this.evnBullets = (EvnBulletsChange)e;
        if (this.currentBullets) this.currentBullets.text = this.evnBullets.showBullets ? this.evnBullets.current.ToString() : "-";
        if (this.totalBullets) this.totalBullets.text = this.evnBullets.showBullets ? this.evnBullets.total.ToString() : "-";
    }

    void OnGrenadesChange(System.Object sender, FrameLord.GameEvent e)
    {
        this.evnGrenades = (EvnGrenadesChange)e;
        if (this.currentGrenades) this.currentGrenades.text = this.evnGrenades.current.ToString();
    }

    void OnWeaponChange(System.Object sender, FrameLord.GameEvent e)
    {
        this.evnWeapon = (EvnWeaponChange)e;
        foreach (GunIcon gi in this.gunIcons)
        {
            gi.icon.SetActive(gi.type == this.evnWeapon.weaponType);
        }
    }

    [System.Serializable]
    public class GunIcon
    {
        public GameObject icon;
        public Weapon.WeaponType type;
    }
}
