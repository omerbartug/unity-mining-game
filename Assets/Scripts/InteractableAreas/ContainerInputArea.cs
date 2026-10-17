using UnityEngine;

// Satılabilir ürünlerin kargo konteynerine aktarılmasını sağlayan etkileşim bölgesidir.
public class ContainerInputArea : MonoBehaviour, IInteractable
{
    public WorkerWorkType WorkType => WorkerWorkType.Operating;
    private CargoContainer container;

    [SerializeField] private float operationTime = 0.15f;

    // Etkileşimin kaç saniye süreceğini belirtir.
    public float OperationTime => operationTime;

    private void Awake()
    {
        container = GetComponentInParent<CargoContainer>();
    }

    // Aktörün envanterinde satılabilir bir ürün ve konteynerde boş yer olup olmadığını doğrular.
    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (container == null) return false;

        // 1. Oyuncu kontrolü (Seçili slot)
        if (inventory is PlayerInventory playerInventory)
        {
            InventoryObject selected = playerInventory.GetSelectedItem();
            if (selected is ItemData itemData && itemData.sellable)
            {
                if (container.CanAdd(itemData, 1))
                {
                    item = itemData;
                    amount = 1;
                    return true;
                }
            }
        }
        // 2. Operatör işçi kontrolü (Girdi haznesi)
        else if (inventory is WorkerInventory workerInventory)
        {
            foreach (var pair in workerInventory.InputItems)
            {
                if (pair.Key is ItemData itemData && itemData.sellable && container.CanAdd(itemData, 1))
                {
                    item = itemData;
                    amount = 1;
                    return true;
                }
            }
        }

        return false;
    }

    // Ürünü envanterden düşüp konteyner deposuna aktarır.
    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (container == null || item == null || inventory == null) return;

        if (container.CanAdd(item, amount))
        {
            int removed = inventory.RemoveItem(item, amount);
            if (removed > 0)
            {
                container.TryAdd(item, removed);
            }
        }
    }

}
