using UnityEngine;

// Maden çıkarma alanıdır. Oyuncu veya işçi burada etkileşime girerek maden toplar.
public class MiningArea : MonoBehaviour, IInteractable
{
    public WorkerWorkType WorkType => WorkerWorkType.Mining;

    [SerializeField] private ItemData rewardItem;
    public ItemData RewardItem => rewardItem;

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;

    // Envanterin bu madeni kabul edip edemeyeceğini doğrular.
    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = rewardItem;
        amount = 1;

        if (rewardItem == null || inventory == null) return false;

        return inventory.CanAccept(rewardItem, 1);
    }

    // Etkileşim süresi tamamlandığında ödül eşyayı envantere ekler.
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        inventory?.AddItem(item, amount);
    }

    // Etkileşim yarıda kesildiğinde ilerleme çubuğunu sıfırlar.
    public void CancelInteract(ProgressBar progress)
    {
        progress?.ResetProgress();
    }
}