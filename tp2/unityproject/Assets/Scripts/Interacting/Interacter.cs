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
    public float raycastIntervalTime = 1f;
    private float currentRaycastTime;

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
        this.fpsCam = this.GetComponentInChildren<Camera>();
        // Get the enemy script if the interacter is an enemy
        if (this.isNPC)
        {
            this.enemy = this.GetComponent<EnemyController>();
            this.enemyCollider = this.GetComponent<CapsuleCollider>();
            this.interactRange = this.enemy.lookRadius;
            this.currentRaycastTime = this.raycastIntervalTime;
        }
    }

    void Update()
    {
        this.currentRaycastTime += Time.deltaTime;
        if ((this.isNPC && this.currentRaycastTime >= this.raycastIntervalTime && this.enemy.TargetIsWithinRange()) || !this.isNPC) {
            this.currentRaycastTime = 0f;
            this.CheckInteractableItem();
        }
    }

    private void CheckInteractableItem()
    {
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
        if (!this.isNPC)
        {
            this.TurnOffPlayerInteractions();
        }
    }

    private void CanInteract(IInteractable interactableObject)
    {
        // Show interacting UI
        InteractType interactType = interactableObject.GetInteractType();

        if (this.isNPC)
        {
            this.CheckEnemyInteractions(interactType);
        }
        else
        {
            this.CheckPlayerInteractions(interactType, interactableObject);
        }
    }

    private Ray GetRaycastRay()
    {
        Vector3 rayOrigin;
        Vector3 rayDirection;

        if (this.isNPC)
        {
            rayOrigin = Helper.GetEnemyRaycastOrigin(this.transform, this.enemyCollider);
            rayDirection = this.transform.forward;
        }
        else
        {
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
        if (this.enemy != null)
        {
            switch (interactType)
            {
                case InteractType.Player:
                    this.enemy.SetPlayerVisibility(true);
                    break;
            }
        }
        else
        {
            Debug.LogWarning("[Interacter] No enemy script for NPC interacter.");
        }
    }

    private void CheckPlayerInteractions(InteractType interactType, IInteractable interactableObject)
    {
        switch (interactType)
        {
            case InteractType.Bomb:
                Bomb bomb = (Bomb)interactableObject;
                if (bomb.IsDefused())
                {
                    this.interactText.text = "This bomb is defused";
                    this.defuseProgressBar.SetActive(false);
                }
                else
                {
                    this.interactText.text = "Hold \"E\" to defuse";
                    this.defuseProgressBar.SetActive(true);
                }
                this.interactTextPanel.SetActive(true);
                // Interact if is player and interacting
                if (ActionMapper.IsInteracting())
                {
                    interactableObject.Interact();
                }
                break;
            case InteractType.AmmoBox:
                AmmoBox ammoBox = (AmmoBox)interactableObject;
                if (ammoBox.IsEmpty())
                {
                    this.interactText.text = "This ammo box is empty";
                }
                else
                {
                    this.interactText.text = "Press \"E\" to get ammo";
                }
                this.interactTextPanel.SetActive(true);
                // Interact if is player and interacting
                if (ActionMapper.IsInteracting())
                {
                    interactableObject.InteractWithCaller(this.gameObject);
                }
                break;
            case InteractType.Weapon:
                WeaponPickup weaponPickup = (WeaponPickup)interactableObject;
                this.interactText.text = "Press \"E\" to pick up " + weaponPickup.GetWeaponLabel();
                this.interactTextPanel.SetActive(true);
                // Interact if is player and interacting
                if (ActionMapper.IsInteracting())
                {
                    interactableObject.InteractWithCaller(this.gameObject);
                }
                break;
            case InteractType.Player:
                this.interactTextPanel.SetActive(false);
                break;
        }
    }

    private void TurnOffPlayerInteractions()
    {
        this.interactTextPanel.SetActive(false);
        this.defuseProgressBar.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, this.interactRange);
    }
}
