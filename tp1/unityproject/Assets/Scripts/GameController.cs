using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // START --> Implementing the Singleton Pattern
    private static GameController _instance;

    public static GameController Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // FINISH --> Implementing the Singleton Pattern

    public GameObject pausePanel;
    // Player prefab used to instantiate the player
    public GameObject playerPrefab;
    // Big asteroid prefab used to instantiate asteroids
    public GameObject bigAsteroidPrefab;
    // Player lives kept here
    public int playerLives = 3;
    // Level of the game
    public int level = 0;
    // Number of asteroids initially, then it's base + level
    public int baseAsteroidsPerLevel = 4;

    [SerializeField]
    private bool isPaused = false;

    void Start()
    {
        // Set position to (0,0,0) initially
        this.transform.position = Vector3.zero;
        // Instantiate the player
        this.instantiatePlayer();
        // Generate the asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    // Update is called once per frame
    void Update()
    {
        this.checkIfPause();
    }

    /* ------------------------- PLAYER GENERATION ------------------------- */

    private void instantiatePlayer() {
        Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
    }

    /* ------------------------- ASTEROID GENERATION ------------------------- */

    private int calculateNumberOfAsteroids() {
        return this.baseAsteroidsPerLevel + this.level;
    }

    // Instantiates numberOfAsteroids asteroids
    private void instantiateAsteroids(int numberOfAsteroids) {
        for (int i = 0; i < numberOfAsteroids; i++) {
            Instantiate(this.bigAsteroidPrefab, Vector3.zero, Quaternion.identity);
        }
    }
    
    /* ------------------------- PAUSE ------------------------- */
    // Checks if the games needs pausing
    private void checkIfPause()
    {
        // Detect PAUSE when the player presses P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (this.isPaused) {
                this.unpause();
            } else {
                this.pause();
            }
        }
    }

    public void pause() {
        this.isPaused = true;
        Time.timeScale = 0f;
        this.pausePanel.SetActive(true);
    }

    public void unpause() {
        this.isPaused = false;
        Time.timeScale = 1f;
        this.pausePanel.SetActive(false);
    }
}
