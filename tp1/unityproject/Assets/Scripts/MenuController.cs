using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set position back to 0,0,0 just in case someone moved it
        this.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadGame() {
        SceneManager.LoadScene("Game Play", LoadSceneMode.Single);
    }
}
