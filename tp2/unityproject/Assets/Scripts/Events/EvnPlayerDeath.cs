public class EvnPlayerDeath : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.PLAYER_DEATH;

    public static EvnPlayerDeath notifier = new EvnPlayerDeath();
    
    public EvnPlayerDeath()
    {
        eventName = EventName;
    }
}
