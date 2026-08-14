using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private ProgressBar progress;
    

    [SerializeField] private PlayerMovement playerMovement;
    



    private IInteractable currentInteractable;
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
            currentInteractable = null;
        }
    }


    private void Update()
    {
       if (currentInteractable != null)
        {
            if (Input.GetKey(KeyCode.E)){
                playerMovement.DisableMovement();
                currentInteractable.Interact(inventory, progress);}
            else{
                playerMovement.EnableMovement();
                currentInteractable.ResetInteract(progress);}
            
        }
        if(Input.GetKeyDown(KeyCode.P)){
            Debug.Log(PlayerStats.Instance.GetPlayerMoney());
        }
    }


}

