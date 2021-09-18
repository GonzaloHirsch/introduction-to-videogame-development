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
        this.checkIfPause();
    }

    // Checks if the games needs pausing
    private void checkIfPause()
    {
        // Detect PAUSE when the player presses P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (this.isPaused)
            {
                this.unpause();
            }
            else
            {
                this.pause();
            }
        }
    }

    public void pause()
    {
        this.isPaused = true;
        Time.timeScale = 0f;
        if (this.pausePanel != null) this.pausePanel.SetActive(true);
    }

    public void unpause()
    {
        this.isPaused = false;
        Time.timeScale = 1f;
        if (this.pausePanel != null) this.pausePanel.SetActive(false);
    }

    public void goToMainMenu()
    {
        // Unpausing the game to not break it
        this.isPaused = false;
        Time.timeScale = 1f;
        // Load the new scene
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
