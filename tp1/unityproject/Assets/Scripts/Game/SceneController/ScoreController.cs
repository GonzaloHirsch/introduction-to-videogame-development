using UnityEngine;
using UnityEngine.UI;

public class ScoreController : FrameLord.MonoBehaviorSingleton<ScoreController>
{
    public Text scoreText;

    void Start() {
        // Update the score to show 0
        this.updateScoreText();
    }

    private void OnScoreChange(System.Object sender, FrameLord.GameEvent e){
        this.addScore(((EvnUpdateScore)e).score);
    }

    // Resets the score and adds the listener
    public void ResetScore() {
        Score.Instance.ResetScore();
        // Start listening for the score event
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnUpdateScore.EventName, OnScoreChange);
    }

    public void addScore(int amount) {
        Score.Instance.AddScore(amount);
        // Call the method to update the score text from the stored instance
        this.updateScoreText();
    }

    public int getScore() {
        return Score.Instance.GetScore();
    }

    // Instance methods
    public void updateScoreText() {
        this.scoreText.text = Score.Instance.GetScore().ToString();
    }
}
