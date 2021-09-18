using System.Collections;
using System.Collections.Generic;

public class EvnUpdateScore : FrameLord.GameEvent
{
    public const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.UPDATE_SCORE;

    public static Dictionary<int, EvnUpdateScore> NotifierMemory = new Dictionary<int, EvnUpdateScore>();

    public int score = 0;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public EvnUpdateScore(int _score)
    {
        eventName = EventName;
        score = _score;
    }

    public static EvnUpdateScore GetNotifier(int score) {
        if (!NotifierMemory.ContainsKey(score)){
            NotifierMemory.Add(score, new EvnUpdateScore(score));   
        }
        return NotifierMemory[score];
    } 
}
