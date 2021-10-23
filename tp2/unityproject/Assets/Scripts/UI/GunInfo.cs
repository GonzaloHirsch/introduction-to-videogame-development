using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInfo : MonoBehaviour
{
    public Text currentBullets;
    public Text totalBullets;
    public Text currentGrenades;
    private EvnBulletsChange evnBullets;
    private EvnGrenadesChange evnGrenades;

    void Awake()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBulletsChange.EventName, OnBulletsChange);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnGrenadesChange.EventName, OnGrenadesChange);
    }

    void OnBulletsChange(System.Object sender, FrameLord.GameEvent e) {
        this.evnBullets = (EvnBulletsChange) e;
        if (this.currentBullets) this.currentBullets.text = this.evnBullets.current.ToString();
        if (this.totalBullets) this.totalBullets.text = this.evnBullets.total.ToString();
    }
    
    void OnGrenadesChange(System.Object sender, FrameLord.GameEvent e) {
        this.evnGrenades = (EvnGrenadesChange) e;
        if (this.currentGrenades) this.currentGrenades.text = this.evnGrenades.current.ToString();
    }
}
