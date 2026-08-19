using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private ProgressBar progress;
    [SerializeField] private PlayerMovement playerMovement;
    
   
    private IInteractable currentInteractable;
    
    private float timer = 0f; 

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
            currentInteractable.CancelInteract(progress);
            timer = 0f;
            
            currentInteractable = null;
        }
    }

    private void Update()
    {
       if (currentInteractable != null)
        {
            
            if (Input.GetKey(KeyCode.E) && currentInteractable.TryGetInteractionData(playerInventory, out ItemData item, out int amount))
            {
                playerMovement.DisableMovement();

                timer += Time.deltaTime;
                progress.SetProgress(timer / currentInteractable.OperationTime);

                if (timer >= currentInteractable.OperationTime)
                {
                    
                    currentInteractable.CompleteInteract(playerInventory, item, amount);
                    
                    timer = 0f;
                    progress.ResetProgress();
                    
                }
            }
            else
            {
                
                playerMovement.EnableMovement();
                
                currentInteractable.CancelInteract(progress);
                timer = 0f;
            }
        }

        
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(PlayerStats.Instance.GetPlayerMoney());
        }
    }
}