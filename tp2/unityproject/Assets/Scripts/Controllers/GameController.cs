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
    private bool bombExploded = false;

    [Header("Enemies")]
    public int[] enemiesPerLevel;

    [Header("Controllers")]
    public TimerController timerController;

    /* ------------------------------ LIFECYCLE ------------------------------ */
    void Awake()
    {
        FrameLord.GameEventDispatcher.Instance.ClearSceneListeners();
        this.SetListeners();
    }

    void Start()
    {
        this.SetActiveBombs();
        this.SetTimer();
    }

    /* ------------------------------ LISTENERS ------------------------------ */

    private void SetListeners()
    {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombDefuse.EventName, OnBombDefuse);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombExplode.EventName, OnBombExplode);
    }

    /* ------------------------------ HANDLERS ------------------------------ */

    private void OnBombDefuse(System.Object sender, FrameLord.GameEvent e)
    {
        this.activeBombs--;
        if (this.activeBombs == 0) {
            this.timerController.StopTimer();
        }
        Debug.Log("BOMB DEFUSED FROM GC");
    }

    private void OnBombExplode(System.Object sender, FrameLord.GameEvent e)
    {
        this.bombExploded = true;
        Debug.Log("BOMB EXPLODED FROM GC");
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
}
