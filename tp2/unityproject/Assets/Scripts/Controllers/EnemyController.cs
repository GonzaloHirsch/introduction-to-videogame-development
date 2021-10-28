using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Degree offset the enemy can be from the player to start shooting
    public float angleDifferenceLimit = 5f;
    // Current angle between enemy rotation and rotation needed to face the enemy
    private float angleDifference = 0f;
    // If player within this distance, enemy will turn to face them
    public float turnRadius = 10f;
    // Only want enemies to attack us if we are within a certain range
    public float lookRadius = 30f;
    // Amount of time the enemy will consider the player visible once 
    // it leaves the lookRadius (as if enemy is still alert and searching)
    public float playerVisibilityTimeLimit = 5f;
    // Current amount of time player is considered visible
    private float currentPlayerVisibilityTime = 0f;
    // Marks if the player is visible to the NPC
    private bool playerIsVisible = false;
    // Need a reference to what we are chasing
    private Transform target;
    // Need a reference to our nav mesh agent to move our enemy
    private NavMeshAgent agent;
    // Reference to the Shooter script
    private Shooter shooter;
    private Shootable shootable;
    private float lastHealth;
    private CapsuleCollider enemyCollider;

    void Start()
    {
        this.target = PlayerManager.Instance.player.transform;
        this.agent = this.GetComponent<NavMeshAgent>();
        this.shooter = this.GetComponent<Shooter>();
        this.shootable = this.GetComponent<Shootable>();
        this.enemyCollider = this.GetComponent<CapsuleCollider>();
        this.lastHealth = this.shootable.maxHealth;
    }

    void Update()
    {
        float distance = Vector3.Distance(this.target.position, this.transform.position);

        if (!this.shootable.IsDead())
        {
            this.HandleEnemyMovement(distance);
            this.HandleEnemyShooting(distance);
        }
        else
        {
            // Disable collider to avoid bothering player movement
            this.enemyCollider.enabled = false;
        }
    }

    //**************************************//
    //***********ENEMY MOVEMENT*************//
    //**************************************//
    void HandleEnemyMovement(float distance)
    {
        // If player is not visible to the NPC do nothing
        if (this.playerIsVisible) {
            if (distance <= this.lookRadius) {
                this.ReactToVisiblePlayer(distance);
            } else {
                this.CheckPlayerVisibilityTime();
            } 
            // Only move vertically once spotted
            this.SetVerticalMovementAnimation();

        } else if (!this.EnemyIsMoving() && distance <= this.turnRadius) {
            this.FaceTarget();
        }
        // Set walking or idle animation
        this.SetMovementAnimation();

        // Player was shot, enemy knows his position
        if (this.shootable.currentHealth < this.lastHealth) {
            this.playerIsVisible = true;
        }
        this.lastHealth = this.shootable.currentHealth;
    }

    void ReactToVisiblePlayer(float distance)
    {
        // Want to start chasing player
        this.agent.SetDestination(this.target.position);
        if (distance <= this.agent.stoppingDistance)
        {
            FaceTarget();
        }
    }

    void FaceTarget()
    {
        Quaternion lookRotation = this.GetLookRotationToPlayer();
        // Update our own rotation with smoothing to point in that direction
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    Quaternion GetLookRotationToPlayer()
    {
        // Get direction to the target
        Vector3 direction = (this.target.position - this.transform.position).normalized;
        // Rotation where we point to that target
        return Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        
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

    void SetMovementAnimation()
    {
        if (this.EnemyIsMoving()) {
            this.shooter.SetWalkAnimation();
        } else {
            this.shooter.SetIdleAnimation();
        }
    }

    void SetVerticalMovementAnimation() 
    {
        // Direction of the player
        Vector3 directionToPlayer = Helper.GetEnemyRaycastDirection(this.transform, this.target);
        // Projection of the direction over the XZ plane
        Vector3 projection = new Vector3(directionToPlayer.x, 0f, directionToPlayer.z);
        // Angle between the direction and the projection
        float yAngle = Vector3.Angle(directionToPlayer, projection);
        // Set the vertical animation of the character
        this.shooter.SetBodyRotationAnimation(-1 * yAngle);
    }

    bool EnemyIsMoving()
    {
        return this.agent.velocity.magnitude > 0;
    }

    public void SetPlayerVisibility(bool playerIsVisible)
    {
        this.playerIsVisible = playerIsVisible;
    }


    //**************************************//
    //***********ENEMY SHOOTING*************//
    //**************************************//
    
    void HandleEnemyShooting(float distance)
    {
        if (this.EnemyCanShoot(distance)) {
            Ray ray = new Ray(
                Helper.GetEnemyRaycastOrigin(this.transform, this.enemyCollider),
                Helper.GetEnemyRaycastDirection(this.transform, this.target)
            );
            // Shoot logic
            this.shooter.Shoot(ray);
            // Trigger animation
            this.shooter.HandleShootAnimation();        
        }
    }

    bool EnemyCanShoot(float distance)
    {
        return this.playerIsVisible 
            && distance < this.shooter.weapon.range
            && this.AngleAllowsShooting();
    }

    bool AngleAllowsShooting()
    {
        Quaternion lookRotation = this.GetLookRotationToPlayer();
        // Angle between how where enemy is and how much enemy needs to rotate to face the player
        this.angleDifference = Quaternion.Angle(this.transform.rotation, lookRotation);
        // Return if player can shoot with the current angle
        return this.angleDifference <= this.angleDifferenceLimit;
    }
}
