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
        if (ActionMapper.GetPaused())
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
        GameStatus.Instance.SetGamePaused(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        if (this.pausePanel != null) this.pausePanel.SetActive(true);
        this.PauseSounds();
    }

    public void Unpause()
    {
        this.isPaused = false;
        GameStatus.Instance.SetGamePaused(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        if (this.pausePanel != null) this.pausePanel.SetActive(false);
        this.UnpauseSounds();
    }

    public void GoToMainMenu()
    {
        // Unpausing the game to not break it
        this.isPaused = false;
        GameStatus.Instance.SetGamePaused(false);
        Time.timeScale = 1f;
        this.UnpauseSounds();
        SceneController.LoadMainMenu();
    }

    private void PauseSounds(){
        AudioListener.volume = 0f;
    }

    private void UnpauseSounds(){
        AudioListener.volume = 1f;
    }
}
