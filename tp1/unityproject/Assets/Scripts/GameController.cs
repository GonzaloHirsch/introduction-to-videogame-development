using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private bool playerPendingSpawn = false;

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
        this.checkIfPlayerSpawn();
    }

    /* ------------------------- GAME LIFECYCLE ------------------------- */

    private void startGame()
    {
        // Set variables to initial values
        activeAsteroids = 0;
        this.level = 0;
        this.playerLives = this.initialPlayerLives;
        // Instantiate the player
        this.instantiatePlayer();
        // Generate the asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    private void startNextLevel()
    {
        // Increment level
        this.level++;
        // Instantiate new asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    /* ------------------------- PLAYER GENERATION ------------------------- */

    private void instantiatePlayer()
    {
        Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
    }

    // Used by the player to notify the controller of it's death
    public void notifyPlayerDeath()
    {
        // Reduce lives
        this.playerLives--;
        // Spawn if it has lives left
        if (playerLives > 0)
        {
            // TRY TO SPAWN PLAYER
            this.playerPendingSpawn = true;
        }
        else
        {
            this.playerPendingSpawn = false;
            // GAME OVER
            Debug.Log("GAME OVER");
        }
    }

    private void checkIfPlayerSpawn()
    {
        if (this.playerPendingSpawn)
        {
            // TRY TO SPAWN PLAYER
            this.trySpawnPlayer();
        }
    }

    private void trySpawnPlayer()
    {
        // Find all important enemies
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(Constants.TAG_ASTEROID);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Constants.TAG_ENEMY);
        // Try all asteroids
        bool canSpawn = this.tryGameobjectListForPlayerSpawn(asteroids);
        // Try for enemies
        if (canSpawn) {
            canSpawn = this.tryGameobjectListForPlayerSpawn(enemies);
        }
        // If in the end it can spawn it
        if (canSpawn) {
            this.instantiatePlayer();
            this.playerPendingSpawn = false;
        }
    }

    private bool tryGameobjectListForPlayerSpawn(GameObject[] goList) {
        bool canSpawn = true;
        foreach (GameObject go in goList) {
            canSpawn = canSpawn && this.isGameobjectOutsidePlayerOriginRange(go);
        }
        return canSpawn;
    }

    // Determines if a gameobject is within the spawning area of the player (which is 0,0,0)
    private bool isGameobjectOutsidePlayerOriginRange(GameObject go)
    {
        return Vector3.Distance(Vector3.zero, go.transform.position) >= Constants.MIN_DISTANCE_FROM_PLAYER;
    }

    /* ------------------------- ASTEROID GENERATION ------------------------- */

    private void checkIfAsteroidSpawn()
    {
        // If there are no more asteroids, move to the next level
        if (activeAsteroids == 0)
        {
            this.startNextLevel();
        }
    }

    private int calculateNumberOfAsteroids()
    {
        return this.baseAsteroidsPerLevel + this.level;
    }

    // Instantiates numberOfAsteroids asteroids
    private void instantiateAsteroids(int numberOfAsteroids)
    {
        // Instantiate all asteroids
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            Instantiate(this.bigAsteroidPrefab);
        }
        // Update the number of active asteroids
        activeAsteroids += numberOfAsteroids;
    }

    public static void ChangeAsteroidCount(int count)
    {
        activeAsteroids += count;
        // Debug.Log(activeAsteroids);
    }

    /* ------------------------- PAUSE ------------------------- */
    // Checks if the games needs pausing
    private void checkIfPause()
    {
        // Detect PAUSE when the player presses P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (this.isPaused)
            {
                this.unpause();
            }
            else
            {
                this.pause();
            }
        }
    }

    public void pause()
    {
        this.isPaused = true;
        Time.timeScale = 0f;
        this.pausePanel.SetActive(true);
    }

    public void unpause()
    {
        this.isPaused = false;
        Time.timeScale = 1f;
        this.pausePanel.SetActive(false);
    }

    public void goToMainMenu() {
        // Unpausing the game to not break it
        this.isPaused = false;
        Time.timeScale = 1f;
        // Load the new scene
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }
}
