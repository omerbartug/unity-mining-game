using UnityEngine;

public class ProcessorInputArea : MonoBehaviour, IInteractable
{
    private AutoProcessor processor;
    private float timer;

    [SerializeField] private float firstInsertTime = 0.6f;
    private bool firstInsertDone = false;
    [SerializeField] private float repeatInsertTime = 0.3f;
    


    private void Awake()
    {
        processor = GetComponentInParent<AutoProcessor>();
    }

    public void Interact(Inventory inventory, ProgressBar progress)
    {
        InventoryObject selectedItem = inventory.GetSelectedItem();
        if(selectedItem == null)
            return;
        

        if(selectedItem is ItemData item){

            if(!item.processable)
                return;
 
            float currentOperationTimer;
            timer += Time.deltaTime;

            if(firstInsertDone){
                currentOperationTimer = repeatInsertTime;
            }
            else{
                currentOperationTimer = firstInsertTime;
            }

            progress.SetProgress(timer / currentOperationTimer);

            if (timer >= currentOperationTimer)
            {
                timer = 0;
                progress.ResetProgress();

                processor.AddInput(inventory, item);
                firstInsertDone = true;
            }
        }

    }

    public void ResetInteract(ProgressBar progress)
    {
        timer = 0;
        progress.ResetProgress();
        firstInsertDone = false;
    }
}