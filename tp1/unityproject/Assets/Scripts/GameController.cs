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
        // Singleton stuff
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        // Script stuff
        MusicController[] musicControllers = GameObject.FindObjectsOfType<MusicController>();
        if (musicControllers.Length > 0) {
            // Take the first one, we expect it to be only one music controller
            this.musicController = musicControllers[0];
        }
    }

    // FINISH --> Implementing the Singleton Pattern

    // Player prefab used to instantiate the player
    public GameObject playerPrefab;
    // Big asteroid prefab used to instantiate asteroids
    public GameObject bigAsteroidPrefab;
    // Small enemy ship prefab used to instantiate small enemies
    public GameObject smallEnemyPrefab;
    // Large enermy ship prefab used to instantiate large enemies
    public GameObject largeEnemyPrefab;
    // Player lives kept here
    public int initialPlayerLives = 3;
    // Number of asteroids initially, then it's base + level
    public int baseAsteroidsPerLevel = 4;
    // Time between each ship appearance
    public float dtBetweenEnemies = 25f;
    // Player lives Sprites
 	public GameObject firstLife;
 	public LifeController firstLifeController;
    // Time transcurred since last enemy ship appearance
    private static float timeSinceLastEnemy = 0f;
    private static int activeEnemies = 0;
    private static int activeAsteroids = 0;
    private static int expectedAsteroidDestructions = 0;
    private static int currentAsteroidDestructions = 0;
    private int level = 0;
    private int playerLives = 0;
    private bool playerPendingSpawn = false;
    private MusicController musicController;

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
        this.checkIfAsteroidSpawn();
        this.checkIfEnemySpawn();
        this.checkIfPlayerSpawn();
    }

    /* ------------------------- GAME LIFECYCLE ------------------------- */

    private void startGame()
    {
        // Set variables to initial values
        activeAsteroids = 0;
        activeEnemies = 0;
        expectedAsteroidDestructions = 0;
        currentAsteroidDestructions = 0;
        this.level = 0;
        this.playerLives = this.initialPlayerLives;
        this.firstLifeController = this.firstLife.GetComponent<LifeController>();
        this.firstLifeController.setLives(this.playerLives);
        // Instantiate the player
        this.instantiatePlayer();
        // Generate the asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    private void startNextLevel()
    {
        // Increment level
        this.level++;
        // Restart enemy counter
        timeSinceLastEnemy = 0f;
        // Instantiate new asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    /* ------------------------- PLAYER GENERATION ------------------------- */

    private void instantiatePlayer()
    {
        Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
    }

    public void addLife() {
        this.playerLives++;
        this.firstLifeController.setLives(this.playerLives);
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
            this.firstLifeController.setLives(this.playerLives);
        }
        else
        {
            this.playerPendingSpawn = false;
            // GAME OVER
            this.gameOver();
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

    private void gameOver() {
        SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
    }

    /* ------------------------- ASTEROID GENERATION ------------------------- */

    private void checkIfAsteroidSpawn()
    {
        // If there are no more asteroids, move to the next level
        if (activeAsteroids == 0 && activeEnemies == 0)
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
        // Update number of expected destructions, from 1 asteroid, 7 destructions are possible
        expectedAsteroidDestructions = (numberOfAsteroids * 7);
        currentAsteroidDestructions = 0;
        GameController.Instance.musicController.updateBgSoundSpeed(((float)currentAsteroidDestructions)/expectedAsteroidDestructions);
    }

    public static void ChangeAsteroidCount(int count)
    {
        activeAsteroids += count;
        // Every time this count changes, it's due to an asteroid destruction
        currentAsteroidDestructions++;
        // Notify the music controller of speed acceleration
        GameController.Instance.musicController.updateBgSoundSpeed(((float)currentAsteroidDestructions)/expectedAsteroidDestructions);
    }

    /* ------------------------- ENEMY GENERATION ------------------------- */

    private void checkIfEnemySpawn()
    {
        // Creates a new enemy if the required time has passed
        if (timeSinceLastEnemy >= this.dtBetweenEnemies) {
            this.instantiateEnemyShip();
            timeSinceLastEnemy = 0f;
        }
        timeSinceLastEnemy += Time.deltaTime;
    }


    // Instantiates the enemy ship
    private void instantiateEnemyShip()
    {
        activeEnemies += 1;
        // If score above limit, small ship. Else, random.
        Constants.ENEMY_SHIP enemyType = 
            ScoreCounter.GetScore() >= Constants.INCREASE_DIFFICULTY_SCORE  
                ? Constants.ENEMY_SHIP.SMALL
                : Utils.GetRandomEnumValue<Constants.ENEMY_SHIP>();
        
        Instantiate(
            enemyType == Constants.ENEMY_SHIP.SMALL
                ? this.smallEnemyPrefab
                : this.largeEnemyPrefab
        );
    }

    public static void ChangeEnemyCount(int count)
    {
        activeEnemies += count;
    }
}
