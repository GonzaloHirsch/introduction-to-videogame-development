using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public float speed = 40f;
    private int speedSign;
    private Vector3 velocity;
    private Constants.SCREEN_BOUNDS startBound;
    private float width;
    private float height;
    private float timeSinceLastDirChange = 0f;
    public float dtBetweenDirChanges = 2f;
    public int scoreValue = 100;
    public bool bulletsAreAccurate = false;
    public float dtBetweenShooting = 1.5f;
    public float timeSinceLastShooting = 0f;
    public GameObject player;

    private AudioSource audioSource;

    // Explosion system
    public GameObject explosionSystem;

    void Awake() {
        this.audioSource = this.GetComponent<AudioSource>();
        findPlayerGameObject();
    }

    void Start()
    {
        setupSpriteSize();
        setupStartingPosition();
        updateVelocityVector();
        this.audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        updatePosition();
        changeDirection();
        shoot();
    }
    void findPlayerGameObject() {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag(Constants.TAG_PLAYER);
        if (playerList.Length > 0) {
            this.player = playerList[0];
        }
    }
    void updatePosition() {
        // Multiply the time with the velocity to know the next position
        transform.position += this.velocity * Time.deltaTime;
    }
   
    void updateVelocityVector() {
        this.velocity = new Vector3(
            this.speedSign * this.speed, 
            this.speed * getYDirection(),
            0
        );
    }
   
    void changeDirection() {
        // Add to the time transcurred since last direction change
        this.timeSinceLastDirChange += Time.deltaTime;
        // If it is time to update the direction, update the velocity and
        // reset the time transcurred counter
        if (this.timeSinceLastDirChange >= this.dtBetweenDirChanges) {
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
        float y = Utils.GetRandomNumInRange(
            getScreenHeightBound(Constants.SCREEN_BOUNDS.LOWER) + this.height, 
            getScreenHeightBound(Constants.SCREEN_BOUNDS.UPPER) - this.height
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
        this.height = spriteRenderer.bounds.size.y;
    }

    void destroyEnemyShip() {
        // Add the score to the counter
        ScoreCounter.AddScore(this.scoreValue);
        // Remove 1 from the active enemy counter
        GameController.ChangeEnemyCount(-1);
        // Create the explosion object
        Instantiate(this.explosionSystem, transform.position, Quaternion.identity);
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
        return Utils.GetRandomEnumValue<Constants.ENEMY_DIRECTION>();
    }
    Constants.SCREEN_BOUNDS getRandomScreenBound() {
        return Utils.GetRandomEnumValue<Constants.SCREEN_BOUNDS>();
    }

    float getScreenHeightBound(Constants.SCREEN_BOUNDS bound) {
        return bound == Constants.SCREEN_BOUNDS.UPPER 
            ? ScreenSize.GetScreenToWorldHeight/2 
            : -ScreenSize.GetScreenToWorldHeight/2;
    }
    float getScreenWidthBound(Constants.SCREEN_BOUNDS bound) {
        return bound == Constants.SCREEN_BOUNDS.UPPER 
            ? ScreenSize.GetScreenToWorldWidth/2 
            : -ScreenSize.GetScreenToWorldWidth/2;
    }

    void shoot() {
       // Add to the time transcurred since last shooting
        this.timeSinceLastShooting += Time.deltaTime;
        // Shoot if time transcurred is greater then the dt
        if (this.timeSinceLastShooting >= this.dtBetweenShooting) {
            if (!this.player) {
                findPlayerGameObject();
            }
            if (this.bulletsAreAccurate && this.player != null) {
                this.shootAccurately();
            } else {
                this.shootRandomly();
            }
            this.timeSinceLastShooting = 0f;
        }
    }

    void shootAccurately() {
        Debug.Log("Shooting Accurately!");
        // Bullet rotation
        Vector3 targetDir = this.player.transform.position - transform.position;
        float rotAngle = Vector3.Angle(Vector3.right, targetDir);
        if (this.player.transform.position.y < transform.position.y) {
            rotAngle = 360 - rotAngle;
        }
        shootBulletAtAngle(rotAngle);
    }

    void shootRandomly() {
        shootBulletAtAngle(Utils.GetRandomNumInRange(0, 360));
    }

    void shootBulletAtAngle(float degreeAngle) {
        float rotRadian = (degreeAngle * Mathf.PI)/180;
        // Bullet position
        Quaternion rotQuaternion = Quaternion.AngleAxis(degreeAngle, Vector3.right);
        Quaternion rotQuaternionOnZ = Quaternion.AngleAxis(degreeAngle, Vector3.forward);
        Vector3 angleVector = new Vector3(Mathf.Cos(rotRadian), Mathf.Sin(rotRadian), 0f) * this.width/1.5f;
        Vector3 position = angleVector + this.transform.position;
        // Get bullet from pool
        ObjectPooler.SharedInstance.ActivatePooledObject(Constants.TAG_ENEMY_BULLET, position, rotQuaternionOnZ);
    }
}
