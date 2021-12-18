using UnityEngine;
using UnityEngine.UI;

public class ScoreController : FrameLord.MonoBehaviorSingleton<ScoreController>
{
    public Text scoreText;

    void Start() {
        // Update the score to show 0
        this.UpdateScoreText();
    }

    private void OnScoreChange(System.Object sender, FrameLord.GameEvent e){
        this.AddScore(((EvnUpdateScore)e).score);
    }

    // Resets the score and adds the listener
    public void ResetScore() {
        Score.Instance.ResetScore();
        // Start listening for the score event
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnUpdateScore.EventName, OnScoreChange);
    }

    public void AddScore(int amount) {
        Score.Instance.AddScore(amount);
        // Call the method to update the score text from the stored instance
        this.UpdateScoreText();
    }

    public int GetScore() {
        return Score.Instance.GetScore();
    }

    // Instance methods
    public void UpdateScoreText() {
        this.scoreText.text = Score.Instance.GetScore().ToString();
    }
}
