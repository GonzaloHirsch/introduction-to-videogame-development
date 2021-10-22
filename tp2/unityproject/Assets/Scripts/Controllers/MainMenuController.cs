using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Start() {
        Cursor.lockState = CursorLockMode.None;
    }
    
    public void LoadLevel(int level) {
        GameStatus.Instance.SetLevel(level);
        SceneController.LoadGame();
    }
}
