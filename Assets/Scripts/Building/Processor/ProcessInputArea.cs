using UnityEngine;

public class ProcessorInputArea : MonoBehaviour, IInteractable
{
    private AutoProcessor processor;

    [SerializeField] private float firstInsertTime = 0.6f;
    [SerializeField] private float repeatInsertTime = 0.15f;
    
    private bool firstInsertDone = false;

   
    public float OperationTime => firstInsertDone ? repeatInsertTime : firstInsertTime;

    private void Awake()
    {
        processor = GetComponentInParent<AutoProcessor>();
    }


    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (inventory is PlayerInventory playerInventory)
        {
            InventoryObject selectedItem = playerInventory.GetSelectedItem();

            if (selectedItem != null && selectedItem is ItemData itemData && itemData.processable)
            {
               
                if (processor.InputQueue.Count < processor.Data.storageCapacity)
                {
                    item = itemData;
                    amount = 1;
                    return true;
                }
            }
        }
        return false;
    }

    
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        
        processor.AddInput(inventory, item);
        firstInsertDone = true; 
    }

    public void CancelInteract(ProgressBar progress)
    {
        firstInsertDone = false;
        progress.ResetProgress();
    }
}