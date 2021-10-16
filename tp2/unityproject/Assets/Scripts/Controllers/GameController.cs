using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private int activeBombs = 0;
    private bool bombExploded = false;

    /* ------------------------------ LIFECYCLE ------------------------------ */
    void Awake() {
        FrameLord.GameEventDispatcher.Instance.ClearSceneListeners();
        this.SetListeners();
    }

    void Start() {
        
    }

    /* ------------------------------ LISTENERS ------------------------------ */

    private void SetListeners() {
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombDefuse.EventName, OnBombDefuse);
        FrameLord.GameEventDispatcher.Instance.AddListener(EvnBombExplode.EventName, OnBombExplode);
    }

    /* ------------------------------ HANDLERS ------------------------------ */

    private void OnBombDefuse(System.Object sender, FrameLord.GameEvent e) {
        this.activeBombs--;
        Debug.Log("BOMB DEFUSED FROM GC");
    }
    
    private void OnBombExplode(System.Object sender, FrameLord.GameEvent e) {
        this.bombExploded = true;
        Debug.Log("BOMB EXPLODED FROM GC");
    }
}
