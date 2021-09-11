using UnityEngine;
public static class Constants
{
    public const float MIN_DISTANCE_FROM_PLAYER = 50f;

    public const int INCREASE_DIFFICULTY_SCORE = 60000;

    public enum SCREEN_BOUNDS {UPPER, LOWER};

    public enum ENEMY_DIRECTION {UP, DOWN, FORWARD};

    public enum ENEMY_SHIP {SMALL, LARGE};

    public const string TAG_ASTEROID = "Asteroid";
    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_PLAYER_BULLET = "Player Bullet";
    public const string TAG_ENEMY_BULLET = "Enemy Bullet";
    public const string TAG_GAME_CONTROLLER = "GameController";

    public static Vector2 DESIGN_SIZE = new Vector2(320, 200);
}
