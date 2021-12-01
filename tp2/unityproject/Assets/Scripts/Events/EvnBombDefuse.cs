public class EvnBombDefuse : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.BOMB_DEFUSE;

    public static EvnBombDefuse notifier = new EvnBombDefuse();
    
    public EvnBombDefuse()
    {
        eventName = EventName;
    }
}
