using UnityEngine;

public class ContainerInputArea : MonoBehaviour, IInteractable
{
    private CargoContainer container;

    [SerializeField] private float firstInsertTime = 0.6f;
    [SerializeField] private float repeatInsertTime = 0.15f;
    private bool firstInsertDone = false;

    public float OperationTime => firstInsertDone ? repeatInsertTime : firstInsertTime;

    private void Awake()
    {
        container = GetComponentInParent<CargoContainer>();
    }

    public bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount)
    {
        item = null;
        amount = 0;

        if (container == null) return false;

        // 1. OYUNCU KONTROLÜ
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
        // 2. İŞÇİ KONTROLÜ (Hem Input hem Output haznesine bakar)
        else if (inventory is WorkerInventory workerInventory)
        {
            // Önce işçinin Input haznesine bak (oyuncunun verdikleri)
            foreach (var pair in workerInventory.InputItems)
            {
                if (pair.Key is ItemData itemData && itemData.sellable && container.CanAdd(itemData, 1))
                {
                    item = itemData;
                    amount = 1;
                    return true;
                }
            }

            // Gerekirse Output haznesine de bak (madenden doğrudan getirdikleri)
            foreach (var pair in workerInventory.OutputItems)
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

    public void CompleteInteract(Inventory inventory, ItemData item, int amount)
    {
        if (container == null) return;

        if (container.TryAdd(item, amount))
        {
            if (inventory is PlayerInventory playerInventory)
            {
                playerInventory.RemoveItem(item, amount);
            }
            else if (inventory is WorkerInventory workerInventory)
            {
                if (workerInventory.InputItems.ContainsKey(item))
                    workerInventory.RemoveFromInput(item, amount);
                else if (workerInventory.OutputItems.ContainsKey(item))
                    workerInventory.RemoveFromOutput(item, amount);
            }

            firstInsertDone = true;
        }
    }

    public void CancelInteract(ProgressBar progress)
    {
        firstInsertDone = false;
        progress?.ResetProgress();
    }
}
