using System.Collections;
using System.Collections.Generic;

public class EvnBulletsChange : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.BULLETS_CHANGE;

    public int current = 0;
    public int total = 0;

    public static EvnBulletsChange notifier = new EvnBulletsChange();
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnBulletsChange()
    {
        eventName = EventName;
    }
}
