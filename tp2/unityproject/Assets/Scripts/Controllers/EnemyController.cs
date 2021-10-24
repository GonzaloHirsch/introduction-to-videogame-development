using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Only want enemies to attack us if we are within a certain range
    public float lookRadius = 10f;
    // Need a reference to what we are chasing
    Transform target;
    // Need a reference to our nav mesh agent to move our enemy
    NavMeshAgent agent;

    void Start()
    {
        this.target = PlayerManager.Instance.player.transform;
        this.agent = GetComponent<NavMeshAgent>();
        
    }

    void Update()
    {
        float distance = Vector3.Distance(this.target.position, this.transform.position);

        if (distance <= this.lookRadius) {
            // Want to start chasing player
            this.agent.SetDestination(this.target.position);

            if (distance <= this.agent.stoppingDistance) {
                // Attack the target
                // Face the target
                FaceTarget();
            }
        }
        
    }

    void FaceTarget()
    {
        // Get direction to the target
        Vector3 direction = (this.target.position - this.transform.position).normalized;
        // Rotation where we point to that target
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Update our own rotation with smoothing to point in that direction
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, lookRadius);
    }
}
