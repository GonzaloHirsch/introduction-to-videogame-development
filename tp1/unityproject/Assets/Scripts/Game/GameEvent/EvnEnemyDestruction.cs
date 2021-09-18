
public class EvnEnemyDestruction : FrameLord.GameEvent
{
    public const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.ENEMY_DESTRUCTION;

    public static EvnEnemyDestruction notifier = new EvnEnemyDestruction();

    public int enemyCountDelta = 0;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnEnemyDestruction()
    {
        eventName = EventName;
    }
}
