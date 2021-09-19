using System.Collections;
using System.Collections.Generic;

public class EvnExtraLife : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.EXTRA_LIFE;

    public static EvnExtraLife notifier = new EvnExtraLife();
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnExtraLife()
    {
        eventName = EventName;
    }
}
