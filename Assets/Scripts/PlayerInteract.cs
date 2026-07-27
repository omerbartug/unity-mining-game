using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private IInteractable currentInteractable;
    [SerializeField] private PlayerMovement playerMovement;
    



    
    private void OnTriggerEnter2D(Collider2D other)
    {
        currentInteractable = other.GetComponent<IInteractable>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentInteractable = null;
    }

    private void Update()
    {
       if (currentInteractable != null)
        {
            if (Input.GetKey(KeyCode.E)){
                playerMovement.DisableMovement();
                currentInteractable.Interact(inventory);}
            else{
                playerMovement.EnableMovement();
                currentInteractable.ResetInteract();}
            
        }
        if(Input.GetKeyDown(KeyCode.P)){
            Debug.Log(PlayerStats.Instance.getPlayerMoney());
        }
    }


}

