using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        // Set position back to 0,0,0 just in case someone moved it
        this.transform.position = Vector3.zero;
    }

    public void LoadGame() {
        SceneManager.LoadScene("Game Play", LoadSceneMode.Single);
    }
}
