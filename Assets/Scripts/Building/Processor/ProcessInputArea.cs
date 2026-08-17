using UnityEngine;

public class ProcessorInputArea : MonoBehaviour, IInteractable
{
    private AutoProcessor processor;
    private float timer;

    [SerializeField] private float firstInsertTime = 0.6f;
    private bool firstInsertDone = false;

    [SerializeField] private float repeatInsertTime = 0.15f;
    


    private void Awake()
    {
        processor = GetComponentInParent<AutoProcessor>();
    }

    public void Interact(Inventory inventory, ProgressBar progress)
    {
        if(inventory is PlayerInventory playerInventory)
        {
            InventoryObject selectedItem = playerInventory.GetSelectedItem();

            if (!(selectedItem is ItemData item))
                return;

            if (!item.processable)
                return;
            
            if (processor.InputQueue.Count >= processor.Data.storageCapacity)
                return;

            ProcessInput(playerInventory, item, progress);
        }
    }

    private void ProcessInput(Inventory inventory, ItemData item, ProgressBar progress)
    {
        timer += Time.deltaTime;

        float operationTime = firstInsertDone
            ? repeatInsertTime
            : firstInsertTime;

        progress.SetProgress(timer / operationTime);

        if (timer >= operationTime)
        {
            timer = 0f;
            progress.ResetProgress();

            processor.AddInput(inventory, item);
            firstInsertDone = true;
        }
    }

    public void ResetInteract(ProgressBar progress)
    {
        timer = 0f;
        progress.ResetProgress();
        firstInsertDone = false;
    }
}