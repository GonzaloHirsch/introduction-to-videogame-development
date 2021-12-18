using UnityEngine;
public static class Constants
{
    public const float MIN_DISTANCE_FROM_PLAYER = 50f;
    public enum SCREEN_BOUNDS {UPPER, LOWER};

    public enum ENEMY_DIRECTION {UP, DOWN, FORWARD};

    public enum ENEMY_SHIP {SMALL, LARGE};

    public enum AUDIO_TYPE {
        PLAYER_MOVE,
        PLAYER_DESTROY,
        SMALL_ASTEROID_DESTROY,
        MEDIUM_ASTEROID_DESTROY,
        LARGE_ASTEROID_DESTROY,
        ENEMY_DESTROY,
        SMALL_ENEMY_THEME,
        LARGE_ENEMY_THEME,
        BULLET_FIRE,
        BG_FIRST_BEAT,
        BG_SECOND_BEAT,
        EXTRA_LIFE
    }

    public const string TAG_ASTEROID = "Asteroid";
    public const string TAG_PLAYER = "Player";
    public const string TAG_ENEMY = "Enemy";
    public const string TAG_PLAYER_BULLET = "Player Bullet";
    public const string TAG_ENEMY_BULLET = "Enemy Bullet";
    public const string TAG_GAME_CONTROLLER = "GameController";

    public static Vector2 DESIGN_SIZE = new Vector2(320, 200);
}
