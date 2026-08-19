using UnityEngine;
public class ProcessArea : MonoBehaviour, IInteractable
{

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;


    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
{
    item = null;
    amount = 0;

    if (inventory is PlayerInventory playerInventory)
    {
        InventoryObject selectedItem = playerInventory.GetSelectedItem();

        if (selectedItem == null)
            return false;

        if (selectedItem is not ItemData itemData)
            return false;

        if (!itemData.processable)
        {
            Debug.Log("bu item islenemez");
            return false;
        }

        item = itemData;
        amount = 1;

        return true;
    }

    return false;
}
    

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {

        inventory.RemoveItem(item , amount);
        inventory.AddItem(item.rewardItem, amount); 
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
    
}