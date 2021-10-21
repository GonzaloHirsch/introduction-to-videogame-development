using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : FrameLord.Singleton<GameStatus>
{
    private bool isGamePaused = false;
    public void SetGamePaused(bool isPaused) {
        this.isGamePaused = isPaused;
    }
    public bool GetGamePaused() {
        return isGamePaused;
    }
    
    private int level = 0;
    public void SetLevel(int level) {
        this.level = level;
    }
    public int GetLevel() {
        return this.level;
    }
}
