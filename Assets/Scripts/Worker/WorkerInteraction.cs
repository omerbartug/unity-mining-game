using UnityEngine;

public class WorkerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private Inventory workerInventory;
    private WorkerMovement workerMovement;
    private ProgressBar progress;
    private Worker worker;

    private float timer;
    
    private bool isInteracting = false; 

    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
        workerMovement = GetComponent<WorkerMovement>();
        progress = GetComponentInChildren<ProgressBar>();
        worker = GetComponent<Worker>();
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
        if (interactable != null && interactable == currentInteractable)
        {
            if (isInteracting)
            {
                currentInteractable.CancelInteract(progress);
                isInteracting = false;
            }
            
            currentInteractable = null;
        }
    }

    private void Update()
    {
        if(currentInteractable != null)
        {
            
            if(workerMovement.HasReachedTarget && 
               currentInteractable.TryGetInteractionData(workerInventory, out ItemData item, out int amount))
            {
                
                isInteracting = true;
                timer += Time.deltaTime * worker.MiningSpeed;
                progress.SetProgress(timer / currentInteractable.OperationTime);

                if (timer >= currentInteractable.OperationTime)
                {
                    currentInteractable.CompleteInteract(workerInventory, item, amount);
                    timer = 0f;
                    progress.ResetProgress();
                    isInteracting = false; 
                }
            }
            else
            {
                
                if (isInteracting)
                {
                    currentInteractable.CancelInteract(progress);
                    timer = 0f;
                    progress.ResetProgress();
                    isInteracting = false;
                }
            }
        }
    }
}