using UnityEngine;
using System.Collections.Generic;

public class WorkerInventory : Inventory
{
    private Worker workerStats;
    
    
    private Dictionary<InventoryObject, int> items = new Dictionary<InventoryObject, int>();

    private void Awake()
    {
        workerStats = GetComponent<Worker>();
    }

   

    public override void AddItem(InventoryObject item, int amount)
    {
        int currentTotal = GetTotalAmount();
        int spaceLeft = workerStats.CarryCapacity - currentTotal;
        
        if (spaceLeft <= 0)
        {
            Debug.Log("İşçinin kapasitesi dolu!");
            return;
        }

        
        int amountToAdd = Mathf.Min(amount, spaceLeft);

        
        if (items.ContainsKey(item))
        {
            items[item] += amountToAdd;
        }
        else
        {
            items.Add(item, amountToAdd);
        }
    }

    public override void RemoveItem(InventoryObject item, int amount)
    {
        if (items.ContainsKey(item))
        {
            items[item] -= amount;
            
            
            if (items[item] <= 0)
            {
                items.Remove(item);
            }
        }
    }

    public override void RemoveAll(InventoryObject item)
    {
        if (items.ContainsKey(item))
        {
            items.Remove(item);
        }
    }

    public override bool HasItem(InventoryObject item, int amount)
    {
        return items.ContainsKey(item) && items[item] >= amount;
    }
    
    public bool IsFull()
    {
        return GetTotalAmount() >= workerStats.CarryCapacity;
    }

    private int GetTotalAmount()
    {
        int total = 0;
        foreach (var amount in items.Values)
        {
            total += amount;
        }
        return total;
    }
}