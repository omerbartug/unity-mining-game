using UnityEngine;

// Ham maddelerin fırına/işlemciye aktarılmasını sağlayan etkileşim bölgesidir.
public class ProcessorInputArea : MonoBehaviour, IInteractable
{
    public WorkerWorkType WorkType => WorkerWorkType.Operating;
    private AutoProcessor processor;

    [SerializeField] private float operationTime = 0.15f;

    // Etkileşimin kaç saniye süreceğini belirtir.
    public float OperationTime => operationTime;

    private void Awake()
    {
        processor = GetComponentInParent<AutoProcessor>();
    }

    // Aktörün envanterinde işlenebilir bir ham madde ve fırın kuyruğunda yer olup olmadığını doğrular.
    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (processor == null || processor.InputQueue.Count >= processor.StorageCapacity)
            return false;

        // 1. Oyuncu kontrolü (Seçili slot)
        if (inventory is PlayerInventory playerInventory)
        {
            InventoryObject selectedItem = playerInventory.GetSelectedItem();
            if (selectedItem is ItemData itemData && itemData.processable)
            {
                item = itemData;
                amount = 1;
                return true;
            }
        }
        // 2. Operatör işçi kontrolü (Girdi haznesi)
        else if (inventory is WorkerInventory workerInventory)
        {
            foreach (var pair in workerInventory.InputItems)
            {
                if (pair.Key is ItemData itemData && itemData.processable)
                {
                    item = itemData;
                    amount = 1;
                    return true;
                }
            }
        }

        return false;
    }

    // Ham maddeyi envanterden eksiltir ve işlemcinin kuyruğuna ekler.
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (processor == null || item == null || inventory == null) 
            return;

        int removed = inventory.RemoveItem(item, amount);
        if (removed > 0)
        {
            processor.AddInput(item, amount);
        }
    }

    // Etkileşim kesildiğinde ilerlemeyi sıfırlar.
    public void CancelInteract(ProgressBar progress)
    {
        progress?.ResetProgress();
    }
}