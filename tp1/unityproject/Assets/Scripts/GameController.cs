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
    public int initialPlayerLives = 3;
    // Number of asteroids initially, then it's base + level
    public int baseAsteroidsPerLevel = 4;

    [SerializeField]
    private bool isPaused = false;
    private static int activeAsteroids = 0;
    private int level = 0;
    private int playerLives = 0;

    void Start()
    {
        // Set position to (0,0,0) initially
        this.transform.position = Vector3.zero;
        // Start the game
        this.startGame();
    }

    // Update is called once per frame
    void Update()
    {
        this.checkIfPause();
        this.checkIfAsteroidSpawn();
    }

    /* ------------------------- GAME LIFECYCLE ------------------------- */

    private void startGame() {
        // Set variables to initial values
        activeAsteroids = 0;
        this.level = 0;
        this.playerLives = this.initialPlayerLives;
        // Instantiate the player
        this.instantiatePlayer();
        // Generate the asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    private void startNextLevel() {
        // Increment level
        this.level++;
        // Instantiate new asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    /* ------------------------- PLAYER GENERATION ------------------------- */

    private void instantiatePlayer() {
        Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
    }

    /* ------------------------- ASTEROID GENERATION ------------------------- */

    private void checkIfAsteroidSpawn() {
        // If there are no more asteroids, move to the next level
        if (activeAsteroids == 0) {
            this.startNextLevel();
        }
    }

    private int calculateNumberOfAsteroids() {
        return this.baseAsteroidsPerLevel + this.level;
    }

    // Instantiates numberOfAsteroids asteroids
    private void instantiateAsteroids(int numberOfAsteroids) {
        // Instantiate all asteroids
        for (int i = 0; i < numberOfAsteroids; i++) {
            Instantiate(this.bigAsteroidPrefab);
        }
        // Update the number of active asteroids
        activeAsteroids += numberOfAsteroids;
    }

    public static void ChangeAsteroidCount(int count) {
        activeAsteroids += count;
        Debug.Log(activeAsteroids);
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
