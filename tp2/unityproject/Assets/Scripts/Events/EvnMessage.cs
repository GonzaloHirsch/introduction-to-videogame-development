public class EvnMessage : FrameLord.GameEvent
{
    public new const FrameLord.GameEvent.Event EventName = FrameLord.GameEvent.Event.MESSAGE;

    public string message;

    public static EvnMessage notifier = new EvnMessage();
    
    public EvnMessage()
    {
        eventName = EventName;
    }
}
