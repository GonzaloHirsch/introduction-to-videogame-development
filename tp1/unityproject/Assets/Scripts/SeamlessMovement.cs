using UnityEngine;

/**
 *  Monobehavior for GameObjects that want to inherit a seamless movement across boundaries
 *  Just drag and drop this script to the Gameobject and it's done.
 */
public class SeamlessMovement : MonoBehaviour
{
    private float width;
    private float height;

    // Activation variables
    public bool isSeamlessInX = true;
    public bool isSeamlessInY = true;

    void Start()
    {
        // Recover Sprite Renderer for size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        // Keep sizes in class
        this.width = spriteRenderer.bounds.size.x;
        this.height = spriteRenderer.bounds.size.y;
    }

    // Called when object stops being visible by crossing screen limits
    void OnBecameInvisible()
    {
        this.UpdateSeamlessPosition();
    }

    // Updates the position of the gameobject to make bounds transition seamless
    public void UpdateSeamlessPosition()
    {
        // Screen limits
        float screenXLimit = Screen.width / 2;
        float screenYLimit = Screen.height / 2;
        // Movement updates
        Vector3 xMovement = Vector3.zero;
        Vector3 yMovement = Vector3.zero;
        // Checking positions
        if (this.isSeamlessInX)
        {
            if (transform.position.x > screenXLimit)
            {
                xMovement -= new Vector3(Screen.width + this.width, 0f, 0f);
            }
            else if (transform.position.x < -screenXLimit)
            {
                xMovement += new Vector3(Screen.width + this.width, 0f, 0f);
            }
        }
        if (this.isSeamlessInY)
        {
            if (transform.position.y > screenYLimit)
            {
                yMovement -= new Vector3(0f, Screen.height + this.height, 0f);
            }
            else if (transform.position.y < -screenYLimit)
            {
                yMovement += new Vector3(0f, Screen.height + this.height, 0f);
            }
        }
        // Updating transform
        transform.position += (xMovement + yMovement);
    }
}