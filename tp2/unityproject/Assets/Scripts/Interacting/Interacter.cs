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

    [Header("UI")]
    public Text interactText;
    public GameObject interactTextPanel;
    public GameObject defuseProgressBar;
    public bool isNPC = false;

    void Start()
    {
        this.fpsCam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        this.CheckInteractableItem();
    }

    private void CheckInteractableItem() {
        // Generate a raycast
        // If NPC -> direction it is facing
        // If player, to the mouse position
         this.ray = this.isNPC 
            ? new Ray(this.transform.position, this.transform.forward)
            : this.fpsCam.ScreenPointToRay(Input.mousePosition);

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
            } else {
                this.interactTextPanel.SetActive(false);
                this.defuseProgressBar.SetActive(false);
            }
        } else {
            this.interactTextPanel.SetActive(false);
            this.defuseProgressBar.SetActive(false);
        }
    }

    private void CanInteract(IInteractable interactableObject) {
        // Show interacting UI
        InteractType interactType = interactableObject.GetInteractType();
        switch(interactType) {
            case InteractType.Bomb:
                this.interactText.text = "Hold \"E\" to defuse";
                this.defuseProgressBar.SetActive(true);
            break;
        }
        this.interactTextPanel.SetActive(true);

        // Interact
        if (ActionMapper.IsInteracting()) {
            interactableObject.Interact();
        }
    }
}
