using UnityEngine;

public class ScaledSprite : MonoBehaviour
{
    void Start()
    {
        Vector3 originalScale = transform.localScale;
        float width = ScreenSize.GetScreenToWorldWidth;
        transform.localScale = originalScale * width / Constants.DESIGN_SIZE.x;
    }
}