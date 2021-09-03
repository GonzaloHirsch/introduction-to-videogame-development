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

    // Movement variables
    private Vector3 _accel;
    private float _accel_module;
    private Vector3 _speed;
    private float _rot;

    //Declare a SpriteRenderer variable to holds our SpriteRenderer component
    private SpriteRenderer sprite; 
    private float _distanceFromCeterToTip;


    // Start is called before the first frame update
    void Start()
    {
        _accel = new Vector3(0f,0f,0f);
        _speed = new Vector3(0f,0f,0f);
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
    }

    void UpdateInput() {
        if (Input.GetKey(LEFT)) {
            _rot = angularVelocity;
        } else if (Input.GetKey(RIGHT)) {
            _rot = -angularVelocity;
        } else {
            _rot = 0;
        }

        if (Input.GetKey(UP)) {
            _accel_module = acceleration;
        } else {
            _accel_module = 0f;
        }

        if (Input.GetKey(DOWN)) {
            resetPlayer();
        }

        if (Input.GetKeyDown(SHOOT)) {
            shoot();
        }
    }

    void UpdateRotation() {
        float dt = Time.deltaTime;
        if(_rot != 0) {
            transform.eulerAngles = transform.eulerAngles + Vector3.forward * dt * _rot;  
        }
    }

    void UpdateAcceleration() {
        float dt = Time.deltaTime;
        
        if (_accel_module != 0) {
            _accel = _accel + transform.right * dt * _accel_module;
        }
        _accel = _accel * 0.8f;
        if (_accel.magnitude < float.Epsilon) {
            _accel = new Vector3(0f,0f,0f);
        }
    }

    void UpdatePosition() {
        float dt = Time.deltaTime;
        // Friccion
        _speed = _speed - _speed * dt * friction;
        // Acceleration force
        _speed = _speed + _accel * dt;

        transform.position = transform.position + _speed * dt;
    }

    void resetPlayer() {
        transform.position = new Vector3(0,0,0);
        _speed = new Vector3(0,0,0);
        _accel = new Vector3(0,0,0);
        transform.right = new Vector3(0,1,0);
    }

    void shoot() {
        
        Vector3 bulletPos = transform.position + transform.right * _distanceFromCeterToTip;
        Instantiate(this.bulletPrefab, bulletPos, transform.rotation);
    }

}
