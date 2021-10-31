using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject controlsPanel;
    public GameObject menuPanel;
    public GameObject planPanel;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        this.SeeMenu();
        this.PlayBgMusic();
    }
    
    public void LoadLevel(int level) {
        GameStatus.Instance.SetLevel(level);
        SceneController.LoadGame();
    }

    public void SeeMenu(){
        this.menuPanel.SetActive(true);
        this.controlsPanel.SetActive(false);
        this.planPanel.SetActive(false);
    }
    
    public void SeeControls(){
        this.menuPanel.SetActive(false);
        this.controlsPanel.SetActive(true);
        this.planPanel.SetActive(false);
    }
    
    public void SeePlan(){
        this.menuPanel.SetActive(false);
        this.controlsPanel.SetActive(false);
        this.planPanel.SetActive(true);
    }

    private void PlayBgMusic() {
        AudioManagerSingleton.Instance.Play(Sounds.AUDIO_TYPE.BG_MUSIC, true);
    }
}
