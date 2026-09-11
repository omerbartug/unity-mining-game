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

            if (!itemData.processable || itemData.rewardItem == null)
            {
                Debug.Log("bu item islenemez");
                return false;
            }

            item = itemData;
            amount = 1;

            return true;
        }
        else if (inventory is WorkerInventory workerInventory)
        {
            foreach (var pair in workerInventory.InputItems)
            {
                if (pair.Key is ItemData inputItem && inputItem.processable && inputItem.rewardItem != null)
                {
                    if (workerInventory.CanAddToOutput(inputItem.rewardItem))
                    {
                        item = inputItem;
                        amount = 1;
                        return true;
                    }
                }
            }

            return false;
        }

        return false;
    }
    

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (inventory is PlayerInventory playerInventory)
        {
            playerInventory.RemoveItem(item, amount);
            playerInventory.AddItem(item.rewardItem, amount);
        }
        else if (inventory is WorkerInventory workerInventory)
        {
            workerInventory.RemoveFromInput(item, amount);
            workerInventory.AddToOutput(item.rewardItem, amount);
        }
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
    
}