using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void LoadLevel(int level) {
        GameStatus.Instance.SetLevel(level);
        SceneController.LoadGame();
    }
}
