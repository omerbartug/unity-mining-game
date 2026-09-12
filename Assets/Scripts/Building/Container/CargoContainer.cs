using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoContainer : Building
{
    private const int MAX_ITEM_TYPES = 3;

    [SerializeField] private int storageCapacity = 30;
    public int StorageCapacity => storageCapacity;

    private Dictionary<ItemData, int> storedItems = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> StoredItems => storedItems;

    public event Action OnStorageChanged;

    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var pair in storedItems) total += pair.Value;
        return total;
    }

    public bool CanAdd(ItemData item, int amount = 1)
    {
        if (item == null || !item.sellable) return false;
        if (GetTotalItemCount() + amount > storageCapacity) return false;

        if (!storedItems.ContainsKey(item) && storedItems.Count >= MAX_ITEM_TYPES)
            return false;

        return true;
    }

    public bool TryAdd(ItemData item, int amount = 1)
    {
        if (!CanAdd(item, amount)) return false;

        if (storedItems.ContainsKey(item))
            storedItems[item] += amount;
        else
            storedItems[item] = amount;

        OnStorageChanged?.Invoke();
        return true;
    }

    public int SellAndClearAll()
    {
        int totalEarnings = 0;
        foreach (var pair in storedItems)
        {
            totalEarnings += pair.Key.sellPrice * pair.Value;
        }

        storedItems.Clear();
        OnStorageChanged?.Invoke();
        return totalEarnings;
    }

    public override void CollectItems(Inventory inventory)
    {
        if (inventory is PlayerInventory playerInventory)
        {
            if (storedItems.Count == 0) return;

            foreach (var pair in storedItems)
            {
                playerInventory.AddItem(pair.Key, pair.Value);
            }

            storedItems.Clear();
            OnStorageChanged?.Invoke();
        }
    }

    public bool TryUpgradeCapacity(int cost = 500, int amount = 10, int maxLimit = 60)
    {
        if (storageCapacity >= maxLimit) return false;
        if (PlayerStats.Instance.GetPlayerMoney() < cost) return false;

        PlayerStats.Instance.RemoveMoney(cost);
        storageCapacity = Mathf.Min(storageCapacity + amount, maxLimit);
        OnStorageChanged?.Invoke();
        return true;
    }
}
