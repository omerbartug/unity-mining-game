using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    public WorkerWorkType WorkType => WorkerWorkType.Mining;

    [SerializeField] private ItemData rewardItem;
    public ItemData RewardItem => rewardItem;

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;
    

    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = rewardItem;
        amount = 1;

        if (rewardItem == null || inventory == null) return false;

        return inventory.CanAccept(rewardItem, 1);
    }

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        inventory?.AddItem(item, amount);
    }

    public void CancelInteract(ProgressBar progress)
    {
        progress.ResetProgress();
    }
}