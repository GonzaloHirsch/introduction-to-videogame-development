using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject bulletPrefab;
    
    // Thrust animation
    public GameObject thrustObject;
    private Animator thrustAnimator;

    // Keyboard keys
    public KeyCode UP = KeyCode.UpArrow;
    public KeyCode DOWN = KeyCode.DownArrow;
    public KeyCode LEFT = KeyCode.LeftArrow;
    public KeyCode RIGHT = KeyCode.RightArrow;
    public KeyCode SHOOT = KeyCode.Space;

    // Ship constants
    public float angularVelocity = 200f;
    public float acceleration = 10000f;
    public float deaccelerationRate = 0.9f;
    public float friction = 1f;

    // Explosion system
    public GameObject explosionSystem;

    // Hyperdrive variables

    [Range(0.0f, 1.0f)]
    public float hyperdriveSuccessPossibility = 0.5f;
    public float hyperdriveCooldownTime = 4.0f;
    private float timeBetweenHyperdrives = 0.0f;

    // Movement variables
    private Vector3 accel;
    private float accelModule;
    private Vector3 speed;
    private float rot;

    // Declare a SpriteRenderer variable to holds our SpriteRenderer component
    private SpriteRenderer sprite;
    private float distanceFromCeterToTip;

    // Cooldown for shooting
    public float shootingCooldown = 0.5f;
    private float timeBetweenShooting = 0f;

    // Recover the instance of the Game Controller to be able to notify
    void Awake()
    {
        this.thrustAnimator = this.thrustObject.GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
        this.accel = new Vector3(0f, 0f, 0f);
        this.speed = new Vector3(0f, 0f, 0f);
        this.sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        this.distanceFromCeterToTip = this.sprite.bounds.size.y;
        // Shooting
        this.timeBetweenShooting = this.shootingCooldown;
        // Hyperdrive
        this.timeBetweenHyperdrives = this.hyperdriveCooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateInput();
        this.UpdateRotation();
        this.UpdateAcceleration();
        this.UpdatePosition();
        // Update hyperdrive cooldown time
        this.timeBetweenHyperdrives += Time.deltaTime;
        // Update shooting cooldown time
        this.timeBetweenShooting += Time.deltaTime;
    }

    void UpdateInput()
    {
        if (Input.GetKey(LEFT))
        {
            this.rot = angularVelocity;
        }
        else if (Input.GetKey(RIGHT))
        {
            this.rot = -angularVelocity;
        }
        else
        {
            this.rot = 0;
        }

        if (Input.GetKey(UP))
        {
            this.accelModule = acceleration;
            this.PlayThrustSound();
            this.thrustAnimator.SetBool("IsAccelerating", true);
        } else { 
            this.accelModule = 0f;
            this.StopThrustSound();
            this.thrustAnimator.SetBool("IsAccelerating", false);
        }

        if (Input.GetKeyDown(DOWN))
        {
            this.TryHyperdrive();
        }

        if (Input.GetKeyDown(SHOOT) && this.timeBetweenShooting >= this.shootingCooldown)
        {
            this.timeBetweenShooting = 0f;
            this.Shoot();
        }
    }

    void UpdateRotation()
    {
        float dt = Time.deltaTime;
        if (this.rot != 0)
        {
            transform.eulerAngles = transform.eulerAngles + Vector3.forward * dt * this.rot;
        }
    }

    void UpdateAcceleration()
    {
        float dt = Time.deltaTime;

        if (this.accelModule != 0)
        {
            this.accel = this.accel + transform.right * dt * this.accelModule;
        }
        this.accel = this.accel * 0.8f;
        if (this.accel.magnitude < float.Epsilon)
        {
            this.accel = new Vector3(0f, 0f, 0f);
        }
    }

    void UpdatePosition()
    {
        float dt = Time.deltaTime;
        // Friccion
        this.speed -= this.speed * dt * friction;
        // Acceleration force
        this.speed += this.accel * dt;

        transform.position = transform.position + this.speed * dt;
    }

    void TryHyperdrive()
    {
        // Check if the cooldown period has passed
        if (this.timeBetweenHyperdrives >= this.hyperdriveCooldownTime)
        {
            // Get random number to determine the hyperdrive success
            float rnd = Utils.GetRandomNumInRange(0.0f, 1.0f);
            if (rnd <= this.hyperdriveSuccessPossibility)
            {
                // Calculate a new position
                float newX = Utils.GetRandomNumInRange(-ScreenSize.GetScreenToWorldWidth / 2, ScreenSize.GetScreenToWorldWidth / 2);
                float newY = Utils.GetRandomNumInRange(-ScreenSize.GetScreenToWorldHeight / 2, ScreenSize.GetScreenToWorldHeight / 2);
                // Set the new variables
                transform.position = new Vector3(newX, newY, 0);
                this.speed = new Vector3(0, 0, 0);
                this.accel = new Vector3(0, 0, 0);
            }
            // Reset the time between hyperdrives
            this.timeBetweenHyperdrives = 0.0f;
        }
    }

    // Destroy the player when it collides with something
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if collided with anything other than a player bullet
        if (!other.gameObject.CompareTag(Constants.TAG_PLAYER_BULLET))
        {
            this.StopThrustSound();
            // Notify the player death
            FrameLord.GameEventDispatcher.Instance.Dispatch(this, EvnPlayerDeath.notifier);
            // Create the explosion object
            Instantiate(this.explosionSystem, transform.position, Quaternion.identity);
            // Destroy the object
            Destroy(this.gameObject);
        }
    }

    void Shoot()
    {
        Vector3 bulletPos = transform.position + transform.right * this.distanceFromCeterToTip;
        ObjectPooler.SharedInstance.ActivatePooledObject(Constants.TAG_PLAYER_BULLET, bulletPos, transform.rotation);
    }

    // Sound methods

    void PlayThrustSound()
    {
        AudioManager.Instance.Play(Constants.AUDIO_TYPE.PLAYER_MOVE, true);
    }

    void StopThrustSound()
    {
        AudioManager.Instance.Stop(Constants.AUDIO_TYPE.PLAYER_MOVE);
    }
}
