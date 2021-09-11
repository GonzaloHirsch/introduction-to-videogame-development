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
        this.width = spriteRenderer.bounds.size.x / 2;
        this.height = spriteRenderer.bounds.size.y / 2;
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
        Vector3 pos = transform.position;
        // Checking positions
        if (this.isSeamlessInX)
        {
            if (transform.position.x > screenXLimit)
            {
                pos.x -= Screen.width + this.width;
            }
            else if (transform.position.x < -screenXLimit)
            {
                pos.x += Screen.width + this.width;
            }
        }
        if (this.isSeamlessInY)
        {
            if (transform.position.y > screenYLimit)
            {
                pos.y -= Screen.height + this.height;
            }
            else if (transform.position.y < -screenYLimit)
            {
                pos.y += Screen.height + this.height;
            }
        }
        // Updating transform
        transform.position = pos;
    }
}