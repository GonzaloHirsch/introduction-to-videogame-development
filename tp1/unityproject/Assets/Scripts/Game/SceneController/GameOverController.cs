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

    public void PlayAgain() {
        SceneManager.LoadScene("Game Play", LoadSceneMode.Single);
    }

    public void MainMenu() {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
