using UnityEngine;
public class ProcessArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;

    [SerializeField] private float operationTime = 2f;

    public void Interact(Inventory inventory, ProgressBar progress)
    {
        if (inventory is PlayerInventory playerInventory)
        {
            InventoryObject selectedItem = playerInventory.GetSelectedItem();

            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem is ItemData item)
            {
                if (!item.processable)
                {
                    Debug.Log("bu item islenemez");
                    return;
                }
                Process(playerInventory, item, progress);
            }
        }


    }
    

    public void ResetInteract(ProgressBar progress)
    {
        operationTimer = 0;
        progress.ResetProgress();
    }

    private void Process(Inventory inventory, ItemData item, ProgressBar progress)
    {
        operationTimer += Time.deltaTime;
        progress.SetProgress(operationTimer / operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progress.ResetProgress();

            inventory.RemoveItem(item, 1);
            inventory.AddItem(item.rewardItem, 1);
        }
    }
    
}