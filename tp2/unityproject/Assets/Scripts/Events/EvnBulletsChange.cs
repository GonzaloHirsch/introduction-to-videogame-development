public class EvnBulletsChange : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.BULLETS_CHANGE;

    public int current = 0;
    public int total = 0;
    public bool showBullets = true;

    public static EvnBulletsChange notifier = new EvnBulletsChange();

    public EvnBulletsChange()
    {
        eventName = EventName;
    }
}
