using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    // START --> Implementing the Singleton Pattern
    private static ScoreCounter _instance;
    private static int nextLifeUp;
    public static int pointsForLifeUp = 10000;


    public static ScoreCounter Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    // FINISH --> Implementing the Singleton Pattern

    private static int score = 0;

    private Text scoreText;
    private static GameController gameController;

    void Start() {
        gameController = GameObject.FindObjectOfType<GameController>();
        nextLifeUp = pointsForLifeUp;
        // Get the text component
        this.scoreText = this.GetComponent<Text>();
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
        ScoreCounter.Instance.updateScoreText();
    }

    public static int GetScore() {
        return score;
    }

    // Instance methods
    public void updateScoreText() {
        this.scoreText.text = score.ToString();
    }
}
