using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        this.scoreText.text = "Score: " + Score.Instance.GetScore();
    }

    public void playAgain() {
        SceneManager.LoadScene("Game Play", LoadSceneMode.Single);
    }

    public void mainMenu() {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
