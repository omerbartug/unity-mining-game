using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    
    [SerializeField] private ItemData rewardItem;
    public ItemData RewardItem => rewardItem;

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;
    

    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (rewardItem == null) return false;

        if (inventory is WorkerInventory workerInventory)
        {
            if (!workerInventory.CanAddToOutput(rewardItem))
            {
                return false;
            }
        }

        item = rewardItem;
        amount = 1;
        return true;
    }

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (inventory is PlayerInventory playerInventory)
        {
            playerInventory.AddItem(item, amount);
        }
        else if (inventory is WorkerInventory workerInventory)
        {
            workerInventory.AddToOutput(item, amount);
        }
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
}