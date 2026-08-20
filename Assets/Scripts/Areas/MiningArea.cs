using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    
    [SerializeField] private ItemData rewardItem;
    public ItemData RewardItem => rewardItem;

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;
    

    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = rewardItem;
        amount = 1;

        if (inventory is WorkerInventory workerInventory && workerInventory.IsFull())
        {
            return false; 
        }

        return true;
    }

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        inventory.AddItem(item, amount);
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
}