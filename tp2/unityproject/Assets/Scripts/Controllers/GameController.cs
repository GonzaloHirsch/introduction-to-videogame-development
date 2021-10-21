using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Bombs")]
    private int activeBombs = 0;
    public int[] activeBombsPerLevel;
    public GameObject[] possibleBombLocations;
    private bool bombExploded = false;

    [Header("Enemies")]
    public int[] enemiesPerLevel;

    /* ------------------------------ LIFECYCLE ------------------------------ */
    void Awake()
    {
        FrameLord.GameEventDispatcher.Instance.ClearSceneListeners();
        this.SetListeners();
    }

    void Start()
    {
        this.SetActiveBombs();
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
        foreach (int i in indexes) {
            this.possibleBombLocations[i].SetActive(true);
        }
    }
}
