using UnityEngine;

public class MenuHelicopterController : MonoBehaviour
{
    public float speed;
    public Vector3 rotationType;

    void Update()
    {
        this.transform.Rotate(this.rotationType * this.speed * Time.deltaTime);
    }
}
