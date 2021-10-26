using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Bombs")]
    private int activeBombs = 0;
    public int[] activeBombsPerLevel;
    public int[] bombTimersPerLevel;
    public GameObject[] possibleBombLocations;
    public GameObject[] bombs;
    public Outline[] bombOutlines;

    [Header("Enemies")]
    public int[] activeEnemiesPerLevel;
    public GameObject[] enemies;
    public Outline[] enemyOutlines;
    private int activeEnemies = 0;

    [Header("Killstreaks")]
    public int killstreakKilledEnemies = 0;
    public float killstreakIntervalTime = 2f;
    private float timeSinceLastKill = 0f;
    public int[] killstreakKills;
    public float[] killstreakTimes;
    private Dictionary<int, float> killstreakCurrentTimes = new Dictionary<int, float>();

    [Header("Controllers")]
    public TimerController timerController;

    /* ------------------------------ LIFECYCLE ------------------------------ */
    void Awake()
    {
        FrameLord.GameEventDispatcher.Instance.ClearSceneListeners();
        this.SetListeners();
        this.SetActiveBombs();
        this.SetActiveEnemies();
        this.SetTimer();
        this.RecoverOutlines();
    }

    void Start()
    {
    }

    void Update() {
        this.timeSinceLastKill = Time.deltaTime;
        this.CheckActiveStreaks();
    }

    /* ------------------------------ LISTENERS ------------------------------ */

    private void SetListeners()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombDefuse.EventName, OnBombDefuse);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombExplode.EventName, OnBombExplode);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnPlayerDeath.EventName, OnPlayerDeath);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnEnemyDeath.EventName, OnEnemyDeath);
    }

    /* ------------------------------ HANDLERS ------------------------------ */

    private void OnBombDefuse(System.Object sender, FrameLord.GameEvent e)
    {
        this.activeBombs--;
        if (this.activeBombs == 0) {
            this.timerController.StopTimer();
            GameStatus.Instance.SetPlayerWon(true);
            GameStatus.Instance.AddCompletedLevel(GameStatus.Instance.GetLevel());
            SceneController.LoadGameOver();
        }
    }

    private void OnEnemyDeath(System.Object sender, FrameLord.GameEvent e)
    {
        this.timeSinceLastKill = 0f;
        // COunt killstreak time
        if (this.timeSinceLastKill >= this.killstreakIntervalTime) {
            this.killstreakKilledEnemies = 1;
        } else {
            this.killstreakKilledEnemies++;
        }
        // Check killstreak
        this.CheckKillstreak();
    }

    private void OnBombExplode(System.Object sender, FrameLord.GameEvent e)
    {
        GameStatus.Instance.SetPlayerWon(false);
        SceneController.LoadGameOver();
    }
    
    private void OnPlayerDeath(System.Object sender, FrameLord.GameEvent e)
    {
        // Mark player as dead
        PlayerManager.Instance.player.GetShooter().SetDead(true);
        GameStatus.Instance.SetPlayerWon(false);
        StartCoroutine(this.finishGameCourotine(5));
    }

    IEnumerator finishGameCourotine(int secs)
    {
        yield return new WaitForSeconds(secs);
        
        SceneController.LoadGameOver();
    }

    /* ------------------------------ BOMBS ------------------------------ */

    void SetActiveBombs() {
        // Mark number of active bombs
        this.activeBombs = this.activeBombsPerLevel[GameStatus.Instance.GetLevel()];
        // Get random indexes
        int[] indexes = Helper.GetUniqueRandomNumbersBetween(0, this.possibleBombLocations.Length, this.activeBombs);
        Bomb currentBomb;
        foreach (int i in indexes) {
            // Set the bomb timer
            currentBomb = this.possibleBombLocations[i].GetComponentInChildren<Bomb>();
            currentBomb.SetTimeToExplode(this.bombTimersPerLevel[GameStatus.Instance.GetLevel()]);
            // Set bomb active
            this.possibleBombLocations[i].SetActive(true);
        }
    }

    void SetTimer() {
        this.timerController.SetTimerLimit(this.bombTimersPerLevel[GameStatus.Instance.GetLevel()]);
    }

    /* ------------------------------ ENEMIES ------------------------------ */

    void SetActiveEnemies() {
        // Mark number of active bombs
        this.activeEnemies = this.activeEnemiesPerLevel[GameStatus.Instance.GetLevel()];
        // Get random indexes
        int[] indexes = Helper.GetUniqueRandomNumbersBetween(0, this.enemies.Length, this.activeEnemies);
        foreach (int i in indexes) {
            // Set bomb active
            this.enemies[i].SetActive(true);
        }
    }

    /* ------------------------------ OBJECTS ------------------------------ */
    
    void RecoverObjects() {
        this.enemies = GameObject.FindGameObjectsWithTag("Enemy");
        this.bombs = GameObject.FindGameObjectsWithTag("Bomb");
    }

    void RecoverOutlines(){
        this.bombOutlines = new Outline[this.bombs.Length];
        for (int i = 0; i < this.bombs.Length; i++) {
            this.bombOutlines[i] = this.bombs[i].GetComponent<Outline>();
            this.bombOutlines[i].OutlineMode = Outline.Mode.OutlineVisible;
        }
        this.enemyOutlines = new Outline[this.enemies.Length];
        for (int i = 0; i < this.enemies.Length; i++) {
            this.enemyOutlines[i] = this.enemies[i].GetComponent<Outline>();
            this.enemyOutlines[i].OutlineWidth = 0f;
            this.enemyOutlines[i].OutlineMode = Outline.Mode.OutlineAll;
        }
    }
    
    /* ------------------------------ STREAKS ------------------------------ */

    void CheckKillstreak(){
        for (int i = 0; i < this.killstreakKills.Length; i++) {
            if (this.killstreakKilledEnemies >= this.killstreakKills[i]) {
                if (i == 0) {
                    this.ActivateEnemyVisionStreak();
                } else if (i == 1) {
                    this.ActivateBombVisionStreak();
                }
                // Set time to live for the streak
                this.killstreakCurrentTimes[i] = this.killstreakTimes[i];
            }
        }
    }

    void CheckActiveStreaks() {
        for (int i = 0; i < this.killstreakKills.Length; i++){
            if (this.killstreakCurrentTimes.ContainsKey(i)) {
                this.killstreakCurrentTimes[i] -= Time.deltaTime;
                if (this.killstreakCurrentTimes[i] <= 0) {
                    if (i == 0) {
                        this.DeactivateEnemyVisionStreak();
                    } else if (i == 1) {
                        this.DeactivateBombVisionStreak();
                    }
                }
            }
        }
    }

    void ActivateBombVisionStreak(){
        for (int i = 0; i < this.bombOutlines.Length; i++){
            this.bombOutlines[i].OutlineMode = Outline.Mode.OutlineAll;
        }
    }
    
    void DeactivateBombVisionStreak(){
        for (int i = 0; i < this.bombOutlines.Length; i++){
            this.bombOutlines[i].OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    void ActivateEnemyVisionStreak(){
        Debug.Log("ENEMY");
        for (int i = 0; i < this.enemyOutlines.Length; i++){
            this.enemyOutlines[i].OutlineWidth = 2f;
        }
    }
    
    void DeactivateEnemyVisionStreak(){
        for (int i = 0; i < this.enemyOutlines.Length; i++){
            this.enemyOutlines[i].OutlineWidth = 0f;
        }
    }

}
