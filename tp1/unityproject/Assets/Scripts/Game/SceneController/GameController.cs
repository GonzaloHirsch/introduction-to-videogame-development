using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : FrameLord.MonoBehaviorSingleton<GameController>
{
    new void Awake()
    {
        // Clear all listeners
        FrameLord.GameEventDispatcher.Instance.ClearSceneListeners();

        MusicController[] musicControllers = GameObject.FindObjectsOfType<MusicController>();
        if (musicControllers.Length > 0) {
            // Take the first one, we expect it to be only one music controller
            this.musicController = musicControllers[0];
        }
    }

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
    // Time transcurred since last enemy ship appearance
    private float timeSinceLastEnemy = 0f;
    private int activeEnemies = 0;
    private int activeAsteroids = 0;
    private int expectedAsteroidDestructions = 0;
    private int currentAsteroidDestructions = 0;
    private int level = 0;
    private int playerLives = 0;
    private bool playerPendingSpawn = false;
    private MusicController musicController;

    // Score
    public int scoreForLifeUp = 10000;
    private int prevScore = 0;

    void Start()
    {
        // Setting the listeners
        this.setListeners();
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
        this.checkScore();
    }

    /* ------------------------- GAME LIFECYCLE ------------------------- */

    private void setListeners() {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnPlayerDeath.EventName, OnPlayerDeath);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnEnemyDestruction.EventName, OnEnemyDestruction);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnAsteroidDestruction.EventName, OnAsteroidDestruction);
    }

    /* ------------------------- GAME LIFECYCLE ------------------------- */

    private void startGame()
    {
        // Set variables to initial values
        activeAsteroids = 0;
        activeEnemies = 0;
        this.expectedAsteroidDestructions = 0;
        currentAsteroidDestructions = 0;
        this.level = 0;
        this.playerLives = this.initialPlayerLives;
        // Score
        this.prevScore = 0;
        // Setting the lives UI
        LifeController.Instance.SetInitialLives(this.playerLives);
        // Instantiate the player
        this.instantiatePlayer();
        // Generate the asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
        // Reset the score
        ScoreController.Instance.ResetScore();
    }

    private void startNextLevel()
    {
        // Increment level
        this.level++;
        // Restart enemy counter
        this.timeSinceLastEnemy = 0f;
        // Instantiate new asteroids
        this.instantiateAsteroids(this.calculateNumberOfAsteroids());
    }

    /* ------------------------- PLAYER GENERATION ------------------------- */

    private void OnPlayerDeath(System.Object sender, FrameLord.GameEvent e){
        // Reduce lives
        this.playerLives--;
        // Spawn if it has lives left
        if (playerLives > 0)
        {
            // Mark the player pending spawn
            this.playerPendingSpawn = true;
        }
        else
        {
            // Game Over
            this.playerPendingSpawn = false;
            this.gameOver();
        }
    }

    private void instantiatePlayer()
    {
        Instantiate(this.playerPrefab, Vector3.zero, Quaternion.identity);
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
        // Load the game over scene
        SceneManager.LoadScene("Game Over", LoadSceneMode.Single);
    }

    /* ------------------------- SCORE ------------------------- */

    private void checkScore() {
        // Recover score from the controller
        int score = Score.Instance.GetScore();
        if (score >= prevScore + this.scoreForLifeUp) {
            // Keep score count
            this.prevScore += this.scoreForLifeUp;
            // Add life
            this.addLife();
        }
    }

    public void addLife() {
        // Increase the lives
        this.playerLives++;
        // Notify the extra life
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnExtraLife.notifier);
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
        // Update number of expected destructions, from 1 asteroid, 7 destructions are possible (2^n - 1) (binary tree formula)
        this.expectedAsteroidDestructions = (numberOfAsteroids * 7);
        // Update the number of active asteroids
        activeAsteroids = this.expectedAsteroidDestructions;
        this.currentAsteroidDestructions = 0;
        GameController.Instance.musicController.updateBgSoundSpeed(((float)this.currentAsteroidDestructions)/this.expectedAsteroidDestructions);
    }

    public void OnAsteroidDestruction(System.Object sender, FrameLord.GameEvent e)
    {
        this.activeAsteroids--;
        // Every time this count changes, it's due to an asteroid destruction
        this.currentAsteroidDestructions++;
        // Notify the music controller of speed acceleration
        GameController.Instance.musicController.updateBgSoundSpeed(((float)this.currentAsteroidDestructions)/this.expectedAsteroidDestructions);
    }
    
    /* ------------------------- ENEMY GENERATION ------------------------- */

    private void checkIfEnemySpawn()
    {
        // Creates a new enemy if the required time has passed
        // And have already passed the first level
        if (this.timeSinceLastEnemy >= this.dtBetweenEnemies) {
            this.instantiateEnemyShip();
            this.timeSinceLastEnemy = 0f;
        }
        this.timeSinceLastEnemy += Time.deltaTime;
    }


    // Instantiates the enemy ship
    private void instantiateEnemyShip()
    {
        activeEnemies += 1;
        // If score above limit, small ship. Else, random.
        Constants.ENEMY_SHIP enemyType = Constants.ENEMY_SHIP.LARGE;
        // Current score
        int score = Score.Instance.GetScore();
        // Begin with only large ships, then random, then only small
        if (score > Constants.MIN_DIFFICULTY_SCORE &&
            score <= Constants.INCREASE_DIFFICULTY_SCORE) {
            enemyType = Utils.GetRandomEnumValue<Constants.ENEMY_SHIP>();
        } else if (score >= Constants.INCREASE_DIFFICULTY_SCORE) {
            enemyType = Constants.ENEMY_SHIP.SMALL;
        }
        // Instantiate the enemy ship  
        Instantiate(
            enemyType == Constants.ENEMY_SHIP.SMALL
                ? this.smallEnemyPrefab
                : this.largeEnemyPrefab
        );
    }

    public void OnEnemyDestruction(System.Object sender, FrameLord.GameEvent e)
    {
        this.activeEnemies--;
    }
}
