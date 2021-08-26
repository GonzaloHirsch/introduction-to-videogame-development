using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float minVelocity = 50f;
    public float maxVelocity = 25f;
    public int rotation;
    public int rotationDelta = 15;
    public int state = 3;
    public int scoreValue = 50;
    public GameObject nextAsteroid;
    public GameObject explosionSystem;

    // PRIVATE VARIABLES
    private GameObject player;
    [SerializeField] private float velocity;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial random velocity between the configured range
        this.velocity = Random.Range(this.minVelocity, this.maxVelocity);
        // Only do this if it is the first state of an asteroid
        if (this.state == 3)
        {
            // Find player to determine position
            // TODO: VER QUE ESTO NO FALLE SI NO HAY PLAYER
            this.player = GameObject.FindGameObjectsWithTag("Player")[0];
            // Initially rotate random degrees
            this.rotation = Random.Range(0, 360);
            transform.eulerAngles = Vector3.forward * this.rotation;
            // Initial random position outside player area
            transform.position = this.GetRandomInitialPosition();
        }
        else
        {
            // Get rotation set by the asteroid parent that created it to be able to give it to the children created
            this.rotation = (int)transform.rotation.eulerAngles.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get delta time
        float dt = Time.deltaTime;
        // Move in the direction it's facing using the transform.right vector
        transform.position += transform.right * dt * this.velocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("TIGGER ENTER");
        if (!other.gameObject.CompareTag("Asteroid")) {
            // Add the score to the counter
            ScoreCounter.AddScore(this.scoreValue);
            // Create the explosion object
            Instantiate(this.explosionSystem, transform.position, Quaternion.identity);
            // If not in the last state, we can spawn the other 2 child asteroids
            if (this.state > 1)
            {
                // Create 2 new asteroids from this one with a slight change in rotation
                Instantiate(this.nextAsteroid, transform.position, Quaternion.Euler(0f, 0f, this.rotation + this.rotationDelta));
                Instantiate(this.nextAsteroid, transform.position, Quaternion.Euler(0f, 0f, this.rotation - this.rotationDelta));
            }
            // In the end we end up destroying it
            Destroy(this.gameObject);
        }
    }

    // CUSTOM FUNCTIONS

    Vector3 GetRandomInitialPosition()
    {
        bool isPositionOk = false;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector3 position = Vector3.zero;
        while (!isPositionOk)
        {
            position = new Vector3(Random.Range(-screenWidth / 2, screenWidth / 2), Random.Range(-screenHeight / 2, screenHeight / 2), 0f);
            isPositionOk = Vector3.Distance(position, this.player.transform.position) >= Constants.MIN_DISTANCE_FROM_PLAYER;
        }
        return position;
    }
}
