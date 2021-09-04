using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject bulletPrefab;

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

    // Hyperdrive variables

    [Range(0.0f, 1.0f)]
    public float hyperdriveSuccessPossibility = 0.5f;
    public float hyperdriveCooldownTime = 4.0f;
    private float timeBetweenHyperdrives = 0.0f;

    // Movement variables
    private Vector3 _accel;
    private float _accel_module;
    private Vector3 _speed;
    private float _rot;

    //Declare a SpriteRenderer variable to holds our SpriteRenderer component
    private SpriteRenderer sprite;
    private float _distanceFromCeterToTip;
    private GameController gameController;

    // Recover the instance of the Game Controller to be able to notify
    void Awake()
    {
        this.gameController = GameObject.FindObjectOfType<GameController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        _accel = new Vector3(0f, 0f, 0f);
        _speed = new Vector3(0f, 0f, 0f);
        sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        _distanceFromCeterToTip = sprite.bounds.size.y;
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
            _rot = angularVelocity;
        }
        else if (Input.GetKey(RIGHT))
        {
            _rot = -angularVelocity;
        }
        else
        {
            _rot = 0;
        }

        if (Input.GetKey(UP))
        {
            _accel_module = acceleration;
        }
        else
        {
            _accel_module = 0f;
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
        if (_rot != 0)
        {
            transform.eulerAngles = transform.eulerAngles + Vector3.forward * dt * _rot;
        }
    }

    void UpdateAcceleration()
    {
        float dt = Time.deltaTime;

        if (_accel_module != 0)
        {
            _accel = _accel + transform.right * dt * _accel_module;
        }
        _accel = _accel * 0.8f;
        if (_accel.magnitude < float.Epsilon)
        {
            _accel = new Vector3(0f, 0f, 0f);
        }
    }

    void UpdatePosition()
    {
        float dt = Time.deltaTime;
        // Friccion
        _speed = _speed - _speed * dt * friction;
        // Acceleration force
        _speed = _speed + _accel * dt;

        transform.position = transform.position + _speed * dt;
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
                float newX = Utils.GetRandomNumInRange(-Screen.width / 2, Screen.width / 2);
                float newY = Utils.GetRandomNumInRange(-Screen.height / 2, Screen.height / 2);
                // Set the new variables
                transform.position = new Vector3(newX, newY, 0);
                _speed = new Vector3(0, 0, 0);
                _accel = new Vector3(0, 0, 0);
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
            // Destroy the object
            Destroy(this.gameObject);
        }
    }

    void shoot()
    {
        Vector3 bulletPos = transform.position + transform.right * _distanceFromCeterToTip;
        Instantiate(this.bulletPrefab, bulletPos, transform.rotation);
    }

}
