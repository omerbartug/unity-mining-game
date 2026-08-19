using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
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

        if (!itemData.sellable)
        {
            Debug.Log("bu item satilmaz");
            return false;
        }

        item = itemData;
        amount = playerInventory.GetSelectedSlot().Amount;

        return true;
    }

    return false;
}
    
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        PlayerStats.Instance.AddMoney(item.sellPrice * amount);
        inventory.RemoveAll(item); 
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
}