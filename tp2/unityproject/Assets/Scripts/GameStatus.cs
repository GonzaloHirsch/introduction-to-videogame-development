using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatus : FrameLord.Singleton<GameStatus>
{
    private const int TOTAL_LEVELS = 3;

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
    public int GetVisibleLevel() {
        return this.level + 1;
    }

    private HashSet<int> completedLevels = new HashSet<int>();
    public void AddCompletedLevel(int completedLevel) {
        this.completedLevels.Add(completedLevel);
    }
    public bool IsGameComplete() {
        return this.completedLevels.Count == TOTAL_LEVELS;
    }

    private bool playerWon = false;
    public void SetPlayerWon(bool playerWon) {
        this.playerWon = playerWon;
    }

    public bool GetPlayerWon() {
        return this.playerWon;
    }
}
