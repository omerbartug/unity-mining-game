using UnityEngine;

// Ham maddelerin işlenerek mamul ürüne dönüştürüldüğü alandır.
public class ProcessArea : MonoBehaviour, IInteractable
{
    public WorkerWorkType WorkType => WorkerWorkType.Processing;

    [SerializeField] private float operationTime = 2f;
    public float OperationTime => operationTime;

    // İşlenebilir eşya ve çıkan ürünün envantere sığıp sığmayacağını doğrular.
    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (inventory is PlayerInventory playerInventory)
        {
            InventorySlot selectedSlot = playerInventory.GetSelectedSlot();
            InventoryObject selectedItem = selectedSlot?.Data;

            if (selectedItem is not ItemData itemData)
                return false;

            if (!itemData.processable || itemData.rewardItem == null)
                return false;

            // Eğer mevcut slot son eşya değilse (tamamen boşalmayacaksa), çıkan ürün için yer var mı kontrol et
            if (selectedSlot.Amount > 1 && !playerInventory.CanAccept(itemData.rewardItem, 1))
                return false;

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
        }

        return false;
    }

    // Ham maddeyi envanterden siler ve üretilen ödül eşyayı ekler.
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (inventory == null || item == null) return;

        inventory.RemoveItem(item, amount);
        inventory.AddItem(item.rewardItem, amount);
    }

    // Etkileşim iptal edildiğinde ilerleme çubuğunu sıfırlar.
    public void CancelInteract(ProgressBar progress)
    {
        progress?.ResetProgress();
    }
}