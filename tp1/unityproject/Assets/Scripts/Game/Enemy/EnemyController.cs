using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public Constants.AUDIO_TYPE audioType;
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
    public float maxDegreeShotRandomization = 20;
    public GameObject player;
    // Explosion system
    public GameObject explosionSystem;

    void Awake()
    {
        this.FindPlayerGameObject();
    }

    void Start()
    {
        this.SetupSpriteSize();
        this.SetupStartingPosition();
        this.UpdateVelocityVector();
        AudioManager.Instance.Play(audioType);
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdatePosition();
        this.ChangeDirection();
        this.Shoot();
    }
    void FindPlayerGameObject()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag(Constants.TAG_PLAYER);
        if (playerList.Length > 0)
        {
            this.player = playerList[0];
        }
    }
    void UpdatePosition()
    {
        // Multiply the time with the velocity to know the next position
        transform.position += this.velocity * Time.deltaTime;
    }

    void UpdateVelocityVector()
    {
        this.velocity = new Vector3(
            this.speedSign * this.speed,
            this.speed * this.GetYDirection(),
            0
        );
    }

    void ChangeDirection()
    {
        // Add to the time transcurred since last direction change
        this.timeSinceLastDirChange += Time.deltaTime;
        // If it is time to update the direction, update the velocity and
        // reset the time transcurred counter
        if (this.timeSinceLastDirChange >= this.dtBetweenDirChanges)
        {
            this.UpdateVelocityVector();
            this.timeSinceLastDirChange = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        this.DestroyEnemyShip();
    }

    void OnBecameInvisible()
    {
        if (this.CheckIfGoalReached())
        {
            this.DestroyEnemyShip(true);
        }
    }

    void SetupStartingPosition()
    {
        // Defines if it will start on the right or left of the screen
        this.startBound = this.GetRandomScreenBound();
        // Indicates the sign the y axis velocity should have
        this.speedSign = this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? -1
            : 1;
        // Adding the sprites width to the x so it is not visible upon creation
        float deltaX = this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? (this.width / 2)
            : -(this.width / 2);

        float x = this.GetScreenWidthBound(this.startBound);
        float y = Utils.GetRandomNumInRange(
            this.GetScreenHeightBound(Constants.SCREEN_BOUNDS.LOWER) + (this.height / 2),
            this.GetScreenHeightBound(Constants.SCREEN_BOUNDS.UPPER) - (this.height / 2)
        );

        transform.position = new Vector3(
            x + deltaX,
            y,
            0
        );
    }
    void SetupSpriteSize()
    {
        // Recover Sprite Renderer for size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Keep sizes in class
        this.width = spriteRenderer.bounds.size.x;
        this.height = spriteRenderer.bounds.size.y;
    }

    void DestroyEnemyShip(bool automaticDestroy = false)
    {
        // Stop the theme of the enemy ship
        AudioManager.Instance.Stop(audioType);
        // Notify destruction of asteroid
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnUpdateScore.GetNotifier(this.scoreValue));
        // Notify enemy destruction
        FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnEnemyDestruction.notifier);
        if (!automaticDestroy)
        {
            // Create the explosion object
            Instantiate(this.explosionSystem, transform.position, Quaternion.identity);
        }
        // Destroy the object when it reaches the other side
        Destroy(this.gameObject);
    }

    bool CheckIfGoalReached()
    {
        // X position of the opposite side of the starting point
        float goalX = -1 * this.GetScreenWidthBound(startBound);
        return this.startBound == Constants.SCREEN_BOUNDS.UPPER
            ? this.transform.position.x <= goalX
            : this.transform.position.x >= goalX;
    }

    float GetYDirection()
    {
        switch (this.GetRandomDirection())
        {
            case Constants.ENEMY_DIRECTION.DOWN:
                return -1;
            case Constants.ENEMY_DIRECTION.UP:
                return 1;
            default:
                return 0;
        }
    }

    Constants.ENEMY_DIRECTION GetRandomDirection()
    {
        return Utils.GetRandomEnumValue<Constants.ENEMY_DIRECTION>();
    }
    Constants.SCREEN_BOUNDS GetRandomScreenBound()
    {
        return Utils.GetRandomEnumValue<Constants.SCREEN_BOUNDS>();
    }

    float GetScreenHeightBound(Constants.SCREEN_BOUNDS bound)
    {
        return bound == Constants.SCREEN_BOUNDS.UPPER
            ? ScreenSize.GetScreenToWorldHeight / 2
            : -ScreenSize.GetScreenToWorldHeight / 2;
    }
    float GetScreenWidthBound(Constants.SCREEN_BOUNDS bound)
    {
        return bound == Constants.SCREEN_BOUNDS.UPPER
            ? ScreenSize.GetScreenToWorldWidth / 2
            : -ScreenSize.GetScreenToWorldWidth / 2;
    }

    void Shoot()
    {
        // Add to the time transcurred since last shooting
        this.timeSinceLastShooting += Time.deltaTime;
        // Shoot if time transcurred is greater then the dt
        if (this.timeSinceLastShooting >= this.dtBetweenShooting)
        {
            if (!this.player)
            {
                this.FindPlayerGameObject();
            }
            if (this.bulletsAreAccurate && this.player != null)
            {
                this.ShootAccurately();
            }
            else
            {
                this.ShootRandomly();
            }
            this.timeSinceLastShooting = 0f;
        }
    }

    void ShootAccurately()
    {
        // Bullet rotation
        Vector3 targetDir = this.player.transform.position - transform.position;
        float rotAngle = Vector3.Angle(Vector3.right, targetDir);
        // Add a certain degree of randomness to avoid instant death
        rotAngle = (rotAngle + Utils.GetRandomNumInRange(
            -this.maxDegreeShotRandomization, 
            this.maxDegreeShotRandomization
        )) % 360;

        if (this.player.transform.position.y < transform.position.y)
        {
            rotAngle = 360 - rotAngle;
        }
        this.ShootBulletAtAngle(rotAngle);
    }

    void ShootRandomly()
    {
        this.ShootBulletAtAngle(Utils.GetRandomNumInRange(0, 360));
    }

    void ShootBulletAtAngle(float degreeAngle)
    {
        float rotRadian = (degreeAngle * Mathf.PI) / 180;
        // Bullet position
        Quaternion rotQuaternion = Quaternion.AngleAxis(degreeAngle, Vector3.right);
        Quaternion rotQuaternionOnZ = Quaternion.AngleAxis(degreeAngle, Vector3.forward);
        Vector3 angleVector = new Vector3(Mathf.Cos(rotRadian), Mathf.Sin(rotRadian), 0f) * this.width / 1.5f;
        Vector3 position = angleVector + this.transform.position;
        // Get bullet from pool
        ObjectPooler.SharedInstance.ActivatePooledObject(Constants.TAG_ENEMY_BULLET, position, rotQuaternionOnZ);
    }
}
