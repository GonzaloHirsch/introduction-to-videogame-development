using UnityEngine;
using UnityEngine.UI;

public class PatrolPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 1);
    }
}
