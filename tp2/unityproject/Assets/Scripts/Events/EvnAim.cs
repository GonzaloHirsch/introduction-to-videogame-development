public class EvnAim : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.AIM;

    public bool isAiming = true;

    public static EvnAim notifier = new EvnAim();

    public EvnAim()
    {
        eventName = EventName;
    }
}
