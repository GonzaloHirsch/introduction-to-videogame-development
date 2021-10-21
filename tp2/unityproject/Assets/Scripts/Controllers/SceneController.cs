using UnityEngine.SceneManagement;
public class SceneController
{
    public static void LoadMainMenu() {
        // Load the new scene
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    
    public static void LoadGame() {
        // Load the new scene
        SceneManager.LoadScene("PlayableLevel", LoadSceneMode.Single);
    }
}
