using UnityEngine;

public class WorkerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private  Inventory workerInventory;
    private WorkerMovement workerMovement;
    private ProgressBar progress;

    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
        workerMovement = GetComponent<WorkerMovement>();
        progress = GetComponentInChildren<ProgressBar>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable == currentInteractable)
        {
            currentInteractable.ResetInteract(progress);
            currentInteractable = null;
        }
    }

    private void Update()
    {
        if(currentInteractable != null && workerMovement.HasReachedTarget)
        {
            Debug.Log("Interact oluyon");
            currentInteractable.Interact(workerInventory, progress);
        }
    }
}