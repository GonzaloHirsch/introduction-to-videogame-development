using UnityEngine;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Text gameOverText;
    public Text gameOverDescription;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (GameStatus.Instance.GetPlayerWon()) {
            if (GameStatus.Instance.IsGameComplete()) {
                // Won game
                this.gameOverText.text = "Congratulations!";
                this.gameOverDescription.text = "You won the game!";
            } else {
                // Just win level
                this.gameOverText.text = "Congratulations!";
                this.gameOverDescription.text = "You won level " + GameStatus.Instance.GetVisibleLevel() + "!";
            } 
        } else {
            // Lose
            this.gameOverText.text = "Game Over";
            this.gameOverDescription.text = "You didn't complete level " + GameStatus.Instance.GetVisibleLevel();
        }
    }

    public void GoToMenu() {
        SceneController.LoadMainMenu();
    }
    
    public void ReplayLevel() {
        SceneController.LoadGame();
    }
}
