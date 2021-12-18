public class EvnGrenadesChange : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.GRENADES_CHANGE;

    public int current = 0;
    public int total = 0;

    public static EvnGrenadesChange notifier = new EvnGrenadesChange();

    public EvnGrenadesChange()
    {
        eventName = EventName;
    }
}
