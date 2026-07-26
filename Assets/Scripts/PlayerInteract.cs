using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    private IInteractable currentInteractable;
    [SerializeField] private PlayerStats player;


    
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
                currentInteractable.Interact(inventory);}
            else{
                currentInteractable.ResetInteract();}
            
        }
        if(Input.GetKeyDown(KeyCode.P)){
            Debug.Log(player.getPlayerMoney());
        }
    }


}

