using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    public GameObject aimObj;
    public GameObject normalAim;
    private EvnAim aimEvent;

    void Start()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnAim.EventName, OnAimChanged);
    }

    void OnAimChanged(System.Object sender, FrameLord.GameEvent e)
    {
        this.aimEvent = (EvnAim)e;
        this.aimObj.SetActive(this.aimEvent.isAiming);
        this.normalAim.SetActive(!this.aimEvent.isAiming);
    }
}
