using System.Collections;
using System.Collections.Generic;

public class EvnWeaponChange : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.WEAPON_CHANGE;

    public Weapon.WeaponType weaponType = Weapon.WeaponType.PISTOL;

    public static EvnWeaponChange notifier = new EvnWeaponChange();
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnWeaponChange()
    {
        eventName = EventName;
    }
}
