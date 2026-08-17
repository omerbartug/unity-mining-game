using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;
    [SerializeField] private float operationTime = 2f;

    public void Interact(Inventory inventory, ProgressBar progress)
    {
        
        if (inventory is PlayerInventory playerInventory)
        {
            InventoryObject selectedItem = playerInventory.GetSelectedItem();
            if (selectedItem == null) return;

            if (selectedItem is ItemData item)
            {
                if (!item.sellable)
                {
                    Debug.Log("Bu urun satilamaz.");
                    return;
                }
                
                
                int amount = playerInventory.GetSelectedSlot().Amount;
                
                
                Sell(inventory, item, amount, progress);
            }
        }
        
    }

    public void ResetInteract(ProgressBar progress)
    {
        operationTimer = 0f;
        progress.ResetProgress();
    }

    private void Sell(Inventory inventory, ItemData item, int amount, ProgressBar progress)
    {
        operationTimer += Time.deltaTime;
        progress.SetProgress(operationTimer / operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progress.ResetProgress();

            
            PlayerStats.Instance.AddMoney(item.sellPrice * amount);
            
            inventory.RemoveAll(item); 
        }
    }
}