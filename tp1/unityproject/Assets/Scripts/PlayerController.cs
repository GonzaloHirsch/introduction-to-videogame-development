using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject map;

    // Keyboard keys
    public KeyCode UP = KeyCode.UpArrow;
    public KeyCode DOWN = KeyCode.DownArrow;
    public KeyCode LEFT = KeyCode.LeftArrow;
    public KeyCode RIGHT = KeyCode.RightArrow;

    // Ship constants
    public float angular_velocity = 200f;
    public float acceleration = 10000f;
    public float deacceleration_rate = 0.9f;
    public float friction = 1f;

    // Movement variables
    private Vector3 _accel;
    private float _accel_module;
    private Vector3 _speed;
    private float _rot;

    // Start is called before the first frame update
    void Start()
    {
        _accel = new Vector3(0f,0f,0f);
        _speed = new Vector3(0f,0f,0f);
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
            _rot = angular_velocity;
        } else if (Input.GetKey(RIGHT)) {
            _rot = -angular_velocity;
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

}
