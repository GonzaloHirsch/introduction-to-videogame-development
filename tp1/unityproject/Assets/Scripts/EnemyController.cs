using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 direction;
    private float speed = 10f;
    private int speedSign;
    private Constants.SCREEN_BOUNDS startBound;
    public int scoreValue = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        destroyEnemyShip();
    }

    void OnBecameInvisible() {
        if (checkIfGoalReached()) {
            destroyEnemyShip();
        }
    }

    void destroyEnemyShip() {
        // Add the score to the counter
        ScoreCounter.AddScore(this.scoreValue);
        // Destroy the object when it reaches the other side
        Destroy(this.gameObject);
    }

    bool checkIfGoalReached() {
        // X position of the opposite side of the starting point
        float goalX = this.getOppositeScreenWidthBound(startBound);
        return this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? this.transform.position.x <= goalX
            : this.transform.position.x >= goalX;
    }

    Vector3 getRandomDirectionVector() {
        float constXSpeed = speed;
        return new Vector3(constXSpeed, 1, 0);
    }

    void setStartingPosition() {
        this.startBound = getRandomScreenBound();
        this.speedSign = this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? -1
            : 1;
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
    float getOppositeScreenWidthBound(Constants.SCREEN_BOUNDS bound) {
        return bound == Constants.SCREEN_BOUNDS.UPPER 
            ? -Screen.width/2 
            : Screen.width/2;
    }
}
