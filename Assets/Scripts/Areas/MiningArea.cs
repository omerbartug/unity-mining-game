using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;

    
    [SerializeField] private ItemData rewardItem;
    public ItemData RewardItem => rewardItem;

    [SerializeField] private float operationTime = 2f;
    

    public void Interact(Inventory inventory, ProgressBar progress)
    {
        operationTimer += Time.deltaTime;
        progress.SetProgress(operationTimer / operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progress.ResetProgress();

            inventory.AddItem(rewardItem, 1);
        }
    }

    public void ResetInteract(ProgressBar progress)
    {
        operationTimer = 0;
        progress.ResetProgress();
    }
}