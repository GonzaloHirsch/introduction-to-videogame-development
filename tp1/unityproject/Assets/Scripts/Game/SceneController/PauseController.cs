using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pausePanel;

    void Start()
    {
        this.transform.position = Vector3.zero;
    }

    void Update()
    {
        this.CheckIfPause();
    }

    // Checks if the games needs pausing
    private void CheckIfPause()
    {
        // Detect PAUSE when the player presses P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (this.isPaused)
            {
                this.Unpause();
            }
            else
            {
                this.Pause();
            }
        }
    }

    public void Pause()
    {
        this.isPaused = true;
        Time.timeScale = 0f;
        if (this.pausePanel != null) this.pausePanel.SetActive(true);
    }

    public void Unpause()
    {
        this.isPaused = false;
        Time.timeScale = 1f;
        if (this.pausePanel != null) this.pausePanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        // Unpausing the game to not break it
        this.isPaused = false;
        Time.timeScale = 1f;
        // Load the new scene
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
