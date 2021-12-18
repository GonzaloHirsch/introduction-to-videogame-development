public class EvnEnemyDeath : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.ENEMY_DEATH;

    public static EvnEnemyDeath notifier = new EvnEnemyDeath();
    
    public EvnEnemyDeath()
    {
        eventName = EventName;
    }
}
