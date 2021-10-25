using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interacter : MonoBehaviour
{
    public float interactRange = 5f;

    [Header("Raycasting")]
    private Ray ray;
    private RaycastHit hit;
    private IInteractable interactableHit;
    private Camera fpsCam;
    private LineRenderer laserLine;

    [Header("UI")]
    public Text interactText;
    public GameObject interactTextPanel;
    public GameObject defuseProgressBar;
    
    [Header("NPC")]
    public bool isNPC = false;
    private EnemyController enemy;
    private CapsuleCollider enemyCollider;

    void Start()
    {
        this.laserLine = this.GetComponent<LineRenderer>();
        this.fpsCam = this.GetComponentInChildren<Camera>();
        // Get the enemy script if the interacter is an enemy
        if (this.isNPC) {
            this.enemy = this.GetComponent<EnemyController>();
            this.enemyCollider = this.GetComponent<CapsuleCollider>();
            this.interactRange = this.enemy.lookRadius;
        }
    }

    void Update()
    {
        this.CheckInteractableItem();
    }

    private void CheckInteractableItem() {
        // Generate a raycast
        // If NPC -> direction it is facing
        // If player, to the mouse position
         this.ray = this.GetRaycastRay();

        this.hit = new RaycastHit();

        // Check if the raycast collided with something
        if (Physics.Raycast(ray, out hit, this.interactRange))
        {
            // Get the component and check if interactable
            this.interactableHit = hit.collider.GetComponent<IInteractable>();
            // Interact if not
            if (this.interactableHit != null)
            {
                this.CanInteract(this.interactableHit);
                this.DebugDrawRay(ray);
                return;
            } 
        } 
        
        this.DebugDrawRay(ray);

        // Disable all text and other interactions
        if (this.isNPC) {
            this.TurnOffEnemyInteractions();
        } else {
            this.TurnOffPlayerInteractions();
        }
    }

    private void CanInteract(IInteractable interactableObject) {
        // Show interacting UI
        InteractType interactType = interactableObject.GetInteractType();
        
        if (this.isNPC) {
            this.CheckEnemyInteractions(interactType);
        } else {
            this.CheckPlayerInteractions(interactType, interactableObject);
        }
    }

    private Ray GetRaycastRay() 
    {
        Vector3 rayOrigin;
        Vector3 rayDirection;

        if (this.isNPC) {
            rayOrigin = this.transform.position;
            // Add the capsule height
            rayOrigin += new Vector3(0, this.enemyCollider.height, 0);
            // Add the capsule radius in the forward direction
            rayOrigin += this.transform.forward * this.enemyCollider.radius;
            // Set the direction
            rayDirection = this.transform.forward;
        }
        else {
            rayOrigin = this.fpsCam.transform.position;
            rayDirection = this.fpsCam.transform.forward;            
        }
        return new Ray(rayOrigin, rayDirection);
    }
    public void DebugDrawRay(Ray ray)
    {
        Debug.DrawRay(ray.origin, ray.direction * this.interactRange, Color.green);
    }

    private void CheckEnemyInteractions(InteractType interactType) 
    {   
        if (this.enemy != null) {
            switch(interactType) {
                case InteractType.Player:
                    this.enemy.setPlayerVisibility(true);
                break;
            }
        } else {
            Debug.LogWarning("[Interacter] No enemy script for NPC interacter.");
        }  
    }

    private void CheckPlayerInteractions(InteractType interactType, IInteractable interactableObject) 
    {
        switch(interactType) {
            case InteractType.Bomb:
                Bomb bomb = (Bomb) interactableObject;
                if (bomb.IsDefused()) {
                    this.interactText.text = "This bomb is defused";
                    this.defuseProgressBar.SetActive(false);
                } else {
                    this.interactText.text = "Hold \"E\" to defuse";
                    this.defuseProgressBar.SetActive(true);
                } 
            break;
            case InteractType.AmmoBox:
                AmmoBox ammoBox = (AmmoBox) interactableObject;
                if (ammoBox.IsEmpty()) {
                    this.interactText.text = "This ammo box is empty";
                } else {
                    this.interactText.text = "Press \"E\" to get ammo";
                }
            break;
        }
        this.interactTextPanel.SetActive(true);

        // Interact if is player and interacting
        if (ActionMapper.IsInteracting()) {
            interactableObject.Interact();
        }
    }

    private void TurnOffPlayerInteractions()
    {
        this.interactTextPanel.SetActive(false);
        this.defuseProgressBar.SetActive(false);
    }

    private void TurnOffEnemyInteractions()
    {
        // this.enemy.setPlayerVisibility(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, this.interactRange);
    }
}
