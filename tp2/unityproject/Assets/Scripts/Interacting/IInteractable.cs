using UnityEngine;

public interface IInteractable
{
    void Interact();

    void InteractWithCaller(GameObject caller);

    InteractType GetInteractType();
}
