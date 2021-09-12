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
        /* if (!spriteRenderer.isVisible) {
            this.UpdateSeamlessPosition();
        } */
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
        float screenXLimit = ScreenSize.GetScreenToWorldWidth / 2;
        float screenYLimit = ScreenSize.GetScreenToWorldHeight / 2;
        float screenWidth = ScreenSize.GetScreenToWorldWidth;
        float screenHeight = ScreenSize.GetScreenToWorldHeight;
        // Movement updates
        Vector3 pos = transform.position;
        // Checking positions
        if (this.isSeamlessInX)
        {
            if (transform.position.x > screenXLimit)
            {
                pos.x -= screenWidth + this.width;
            }
            else if (transform.position.x < -screenXLimit)
            {
                pos.x += screenWidth + this.width;
            }
        }
        if (this.isSeamlessInY)
        {
            if (transform.position.y > screenYLimit)
            {
                pos.y -= screenHeight + this.height;
            }
            else if (transform.position.y < -screenYLimit)
            {
                pos.y += screenHeight + this.height;
            }
        }
        // Updating transform
        transform.position = pos;
    }
}