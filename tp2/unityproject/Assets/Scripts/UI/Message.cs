using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    public Text messageText;
    public float messageTime = 3f;
    private bool isMessageActive = true;
    private float currentMessageTime = 0f;
    private EvnMessage messageEvent;
    private Color transparentColor = new Color(1f, 1f, 1f, 0f);

    void Start()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnMessage.EventName, OnMessageReceived);
        // Make transparent by default
        this.messageText.color = this.transparentColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isMessageActive)
        {
            this.currentMessageTime += Time.deltaTime;
            if (this.currentMessageTime >= this.messageTime)
            {
                this.isMessageActive = false;
                this.currentMessageTime = 0f;
                this.messageText.color = this.transparentColor;
            }
        }
    }

    void OnMessageReceived(System.Object sender, FrameLord.GameEvent e)
    {
        this.messageEvent = (EvnMessage)e;
        // Set the text
        this.messageText.text = this.messageEvent.message;
        // Mark as active
        this.isMessageActive = true;
        // Cue opacity, from transparent to visible
        this.messageText.color = Color.white;
    }
}
