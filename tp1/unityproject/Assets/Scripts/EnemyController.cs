using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 40f;
    private int speedSign;
    private Vector3 velocity;
    private Constants.SCREEN_BOUNDS startBound;
    public float dtBetweenDirChanges = 2f;
    private float timeSinceLastDirChange = 0f;
    public int scoreValue = 100;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        setupSpriteSize();
        setupStartingPosition();
        updateVelocityVector();
    }

    // Update is called once per frame
    void Update()
    {
        // Get delta time
        float dt = Time.deltaTime;
        // Multiply the time with the velocity to know the next position
        transform.position += this.velocity * dt;
        // Add to the time transcurred since last direction change
        this.timeSinceLastDirChange += dt;
        // If it is time to update the direction, update the velocity and
        // reset the time transcurred counter
        if (timeSinceLastDirChange >= dtBetweenDirChanges) {
            updateVelocityVector();
            this.timeSinceLastDirChange = 0;
        }
        
    }
    void OnTriggerEnter2D(Collider2D col) {
        destroyEnemyShip();
    }

    void OnBecameInvisible() {
        if (checkIfGoalReached()) {
            destroyEnemyShip();
        }
    }

    void setupStartingPosition() {
        // Defines if it will start on the right or left of the screen
        this.startBound = getRandomScreenBound();
        // Indicates the sign the y axis velocity should have
        this.speedSign = this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? -1
            : 1;
        // Adding the sprites width to the x so it is not visible upon creation
        float deltaX = this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? this.width
            : -this.width;
        
        float x = getScreenWidthBound(this.startBound);
        float y = Utils.getRandomNumInRange(
            getScreenHeightBound(Constants.SCREEN_BOUNDS.LOWER), 
            getScreenHeightBound(Constants.SCREEN_BOUNDS.UPPER)
        );

        transform.position = new Vector3(
            x + deltaX, 
            y,
            0
        );
    }
    void setupSpriteSize() {
        // Recover Sprite Renderer for size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Keep sizes in class
        this.width = spriteRenderer.bounds.size.x;
    }

    void destroyEnemyShip() {
        // Add the score to the counter
        // ScoreCounter.AddScore(this.scoreValue);
        // Destroy the object when it reaches the other side
        Destroy(this.gameObject);
    }

    bool checkIfGoalReached() {
        // X position of the opposite side of the starting point
        float goalX = -1 * this.getScreenWidthBound(startBound);
        return this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? this.transform.position.x <= goalX
            : this.transform.position.x >= goalX;
    }

    void updateVelocityVector() {
        this.velocity = new Vector3(
            this.speedSign * this.speed, 
            this.speed * getYDirection(),
            0
        );
    }

    float getYDirection() {
        switch(getRandomDirection()) {
            case Constants.ENEMY_DIRECTION.DOWN:
                return -1;
            case Constants.ENEMY_DIRECTION.UP:
                return 1;
            default:
                return 0;
        }
    }

    Constants.ENEMY_DIRECTION getRandomDirection() {
        return Utils.getRandomEnumValue<Constants.ENEMY_DIRECTION>();
    }
    Constants.SCREEN_BOUNDS getRandomScreenBound() {
        return Utils.getRandomEnumValue<Constants.SCREEN_BOUNDS>();
    }

    float getScreenHeightBound(Constants.SCREEN_BOUNDS bound) {
        return bound == Constants.SCREEN_BOUNDS.UPPER 
            ? Screen.height/2 
            : -Screen.height/2;
    }
    float getScreenWidthBound(Constants.SCREEN_BOUNDS bound) {
        return bound == Constants.SCREEN_BOUNDS.UPPER 
            ? Screen.width/2 
            : -Screen.width/2;
    }
}
