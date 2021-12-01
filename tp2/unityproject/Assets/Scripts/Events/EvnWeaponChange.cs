public class EvnWeaponChange : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.WEAPON_CHANGE;

    public Weapon.WeaponType weaponType = Weapon.WeaponType.PISTOL;

    public static EvnWeaponChange notifier = new EvnWeaponChange();
    
    public EvnWeaponChange()
    {
        eventName = EventName;
    }
}
