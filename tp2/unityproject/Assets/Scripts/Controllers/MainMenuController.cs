using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public GameObject controlsPanel;
    public GameObject menuPanel;

    void Start() {
        Cursor.lockState = CursorLockMode.None;
        this.SeeMenu();
    }
    
    public void LoadLevel(int level) {
        GameStatus.Instance.SetLevel(level);
        SceneController.LoadGame();
    }

    public void SeeMenu(){
        this.menuPanel.SetActive(true);
        this.controlsPanel.SetActive(false);
    }
    
    public void SeeControls(){
        this.menuPanel.SetActive(false);
        this.controlsPanel.SetActive(true);
    }
}
