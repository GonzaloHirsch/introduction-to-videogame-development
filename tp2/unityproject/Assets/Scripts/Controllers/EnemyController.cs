using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Only want enemies to attack us if we are within a certain range
    public float lookRadius = 10f;
    // Amount of time the enemy will consider the player visible once 
    // it leaves the lookRadius (as if enemy is still alert and searching)
    public float playerVisibilityTimeLimit = 5f;
    // Current amount of time player is considered visible
    private float currentPlayerVisibilityTime = 0f;
    // Need a reference to what we are chasing
    private Transform target;
    // Need a reference to our nav mesh agent to move our enemy
    private NavMeshAgent agent;
    // Marks if the player is visible to the NPC
    private bool playerIsVisible = false;

    void Start()
    {
        this.target = PlayerManager.Instance.player.transform;
        this.agent = GetComponent<NavMeshAgent>();  
    }

    void Update()
    {
        // If player is not visible to the NPC do nothing
        if (!this.playerIsVisible) {
            return;
        }

        // If visible, get closer
        float distance = Vector3.Distance(this.target.position, this.transform.position);

        if (distance <= this.lookRadius) {
            this.UpdateEnemyPosition(distance);
        } else {
            this.CheckPlayerVisibilityTime();
        } 
    }

    void UpdateEnemyPosition(float distance)
    {
        // Want to start chasing player
        this.agent.SetDestination(this.target.position);
        if (distance <= this.agent.stoppingDistance) {
            // Attack the target
            // Face the target
            FaceTarget();
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

    void CheckPlayerVisibilityTime()
    {
        // If distance is greater than look radius for a 
        // period of time, set player visibility to false
        bool playerVisibilityTimeReached = 
            this.currentPlayerVisibilityTime >= this.playerVisibilityTimeLimit;
        // Reset the timer or keep adding to it
        this.currentPlayerVisibilityTime = playerVisibilityTimeReached
            ? 0f
            : this.currentPlayerVisibilityTime + Time.deltaTime;
        // Player no longer visible if time limit reached
        this.playerIsVisible = !playerVisibilityTimeReached;  
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, lookRadius);
    }

    public void setPlayerVisibility(bool playerIsVisible)
    {
        this.playerIsVisible = playerIsVisible;
    }
}
