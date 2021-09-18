using UnityEngine;
using UnityEngine.UI;

public class ScoreController : FrameLord.MonoBehaviorSingleton<ScoreController>
{
    private static int nextLifeUp;
    public static int pointsForLifeUp = 10000;

    private static int score = 0;

    public Text scoreText;
    private static GameController gameController;

    void Start() {
        gameController = GameController.Instance;
        nextLifeUp = pointsForLifeUp;
        // Reset the score to 0
        ResetScore();
        // Update the score to show 0
        this.updateScoreText();
    }

    // Static Methods to be used from outside the class
    public static void ResetScore() {
        score = 0;
    }

    public static void AddScore(int amount) {
        score += amount;
        if (score > nextLifeUp) {
            gameController.addLife();
            nextLifeUp = nextLifeUp + pointsForLifeUp;
        }
        // Call the method to update the score text from the stored instance
        ScoreController.Instance.updateScoreText();
    }

    public static int GetScore() {
        return score;
    }

    // Instance methods
    public void updateScoreText() {
        this.scoreText.text = score.ToString();
    }
}
