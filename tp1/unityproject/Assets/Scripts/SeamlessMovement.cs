using UnityEngine;

/**
 *  Monobehavior for GameObjects that want to inherit a seamless movement across boundaries
 *  Just drag and drop this script to the Gameobject and it's done.
 */
public class SeamlessMovement : MonoBehaviour
{
    private float width;
    private float height;
    private float screenXLimit;
    private float screenYLimit;
    private float screenWidth;
    private float screenHeight;

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
        // Screen limits
        this.screenXLimit = ScreenSize.GetScreenToWorldWidth / 2;
        this.screenYLimit = ScreenSize.GetScreenToWorldHeight / 2;
        this.screenWidth = ScreenSize.GetScreenToWorldWidth;
        this.screenHeight = ScreenSize.GetScreenToWorldHeight;
    }

    void Update()
    {
        this.UpdateSeamlessPosition();
    }

    // Updates the position of the gameobject to make bounds transition seamless
    public void UpdateSeamlessPosition()
    {
        // Movement updates
        Vector3 xMovement = Vector3.zero;
        Vector3 yMovement = Vector3.zero;
        bool isMoved = false;
        // Checking positions
        if (this.isSeamlessInX)
        {
            if (transform.position.x > (screenXLimit + width))
            {
                xMovement -= new Vector3(screenWidth + this.width * 1.5f, 0f, 0f);
                isMoved = true;
            }
            else if (transform.position.x < -(screenXLimit + width))
            {
                xMovement += new Vector3(screenWidth + this.width * 1.5f, 0f, 0f);
                isMoved = true;
            }
        }
        if (this.isSeamlessInY)
        {
            if (transform.position.y > (screenYLimit + height))
            {
                yMovement -= new Vector3(0f, screenHeight + this.height * 1.5f, 0f);
                isMoved = true;
            }
            else if (transform.position.y < -(screenYLimit + height))
            {
                yMovement += new Vector3(0f, screenHeight + this.height * 1.5f, 0f);
                isMoved = true;
            }
        }
        // Updating transform
        if (isMoved) transform.position += (xMovement + yMovement);
    }
}