using System.Collections;
using System.Collections.Generic;

public class EvnBombExplode : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.BOMB_EXPLODE;

    public static EvnBombExplode notifier = new EvnBombExplode();
    
    public EvnBombExplode()
    {
        eventName = EventName;
    }
}
