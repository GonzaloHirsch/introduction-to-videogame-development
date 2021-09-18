
public class EvnAsteroidDestruction : FrameLord.GameEvent
{
    public const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.ASTEROID_DESTRUCTION;

    public static EvnAsteroidDestruction notifier = new EvnAsteroidDestruction();
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnAsteroidDestruction()
    {
        eventName = EventName;
    }
}
