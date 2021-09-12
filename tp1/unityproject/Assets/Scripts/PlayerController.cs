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

    // Sound variables
    public AudioSource audioSource;

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

    //Declare a SpriteRenderer variable to holds our SpriteRenderer component
    private SpriteRenderer sprite;
    private float distanceFromCeterToTip;
    private GameController gameController;

    // Recover the instance of the Game Controller to be able to notify
    void Awake()
    {
        this.gameController = GameObject.FindObjectOfType<GameController>();
        this.audioSource = this.GetComponent<AudioSource>();
        this.thrustAnimator = this.thrustObject.GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {
        this.accel = new Vector3(0f, 0f, 0f);
        this.speed = new Vector3(0f, 0f, 0f);
        this.sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        this.distanceFromCeterToTip = this.sprite.bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
        UpdateRotation();
        UpdateAcceleration();
        UpdatePosition();
        // Update hyperdrive cooldown time
        this.timeBetweenHyperdrives += Time.deltaTime;
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
            this.playThrustSound();
            this.thrustAnimator.SetBool("IsAccelerating", true);
        }
        else
        {
            this.accelModule = 0f;
            this.stopThrustSound();
            this.thrustAnimator.SetBool("IsAccelerating", false);
        }

        if (Input.GetKeyDown(DOWN))
        {
            tryHyperdrive();
        }

        if (Input.GetKeyDown(SHOOT))
        {
            shoot();
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

    void tryHyperdrive()
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
            // Notify the gamecontroller of the death
            this.gameController.notifyPlayerDeath();
            // Create the explosion object
            Instantiate(this.explosionSystem, transform.position, Quaternion.identity);
            // Destroy the object
            Destroy(this.gameObject);
        }
    }

    void shoot()
    {
        Vector3 bulletPos = transform.position + transform.right * this.distanceFromCeterToTip;
        ObjectPooler.SharedInstance.ActivatePooledObject(Constants.TAG_PLAYER_BULLET, bulletPos, transform.rotation);
    }

    // Sound methods

    void playThrustSound()
    {
        if (!this.audioSource.isPlaying)
        {
            this.audioSource.Play();
        }
    }

    void stopThrustSound()
    {
        if (this.audioSource.isPlaying)
        {
            this.audioSource.Stop();
        }
    }
}
