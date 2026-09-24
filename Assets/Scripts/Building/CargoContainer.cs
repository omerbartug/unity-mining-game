using System;
using System.Collections.Generic;
using UnityEngine;

// Satılabilir ürünleri depolayan ve periyodik sevkiyatlarla satışını sağlayan depolama binasıdır.
public class CargoContainer : Building
{
    private const int MAX_ITEM_TYPES = 3;

    [SerializeField] private int storageCapacity = 30;
    public int StorageCapacity => storageCapacity;

    [Header("Upgrade Settings")]
    [SerializeField] private int upgradeCost = 500;
    [SerializeField] private int upgradeAmount = 10;
    [SerializeField] private int maxCapacityLimit = 60;

    public int UpgradeCost => upgradeCost;
    public int UpgradeAmount => upgradeAmount;
    public int MaxCapacityLimit => maxCapacityLimit;
    public bool IsMaxCapacity => storageCapacity >= maxCapacityLimit;

    private Dictionary<ItemData, int> storedItems = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> StoredItems => storedItems;

    public event Action OnStorageChanged;

    private void OnEnable()
    {
        ShipmentManager.Register(this);
    }

    private void OnDisable()
    {
        ShipmentManager.Unregister(this);
    }

    // Depodaki toplam eşya adedini hesaplar.
    public int GetTotalItemCount()
    {
        int total = 0;
        foreach (var pair in storedItems) total += pair.Value;
        return total;
    }

    // Belirtilen eşyanın konteynere sığıp sığmayacağını doğrular.
    public bool CanAdd(ItemData item, int amount = 1)
    {
        if (item == null || !item.sellable) return false;
        if (GetTotalItemCount() + amount > storageCapacity) return false;

        if (!storedItems.ContainsKey(item) && storedItems.Count >= MAX_ITEM_TYPES)
            return false;

        return true;
    }

    // Eşyayı depoya ekler.
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

    // Sevkiyat anında tüm ürünleri satar, toplam kazancı döner ve depoyu sıfırlar.
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

    // Depodaki eşyaları alabilen envantere (Oyuncu veya İşçi) kayıpsız şekilde aktarır.
    public override void CollectItems(Inventory inventory)
    {
        if (storedItems.Count == 0 || inventory == null) return;

        var itemsToCollect = new List<KeyValuePair<ItemData, int>>(storedItems);
        bool changed = false;

        foreach (var pair in itemsToCollect)
        {
            if (inventory.CanAccept(pair.Key, 1))
            {
                int added = inventory.AddItem(pair.Key, pair.Value);
                if (added > 0)
                {
                    storedItems[pair.Key] -= added;
                    if (storedItems[pair.Key] <= 0)
                    {
                        storedItems.Remove(pair.Key);
                    }
                    changed = true;
                }
            }
        }

        if (changed)
        {
            OnStorageChanged?.Invoke();
        }
    }

    // Oyuncunun parası yeterliyse konteyner depolama kapasitesini artırır.
    public bool TryUpgradeCapacity()
    {
        return TryUpgradeCapacity(upgradeCost, upgradeAmount, maxCapacityLimit);
    }

    // Dışarıdan özel parametrelerle kapasite artırımı yapmayı sağlar.
    public bool TryUpgradeCapacity(int cost, int amount, int maxLimit)
    {
        if (storageCapacity >= maxLimit) return false;
        if (!PlayerStats.Instance.TrySpendMoney(cost)) return false;

        storageCapacity = Mathf.Min(storageCapacity + amount, maxLimit);
        OnStorageChanged?.Invoke();
        return true;
    }
}
