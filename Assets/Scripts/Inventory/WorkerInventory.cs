using UnityEngine;
using System;
using System.Collections.Generic;

public class WorkerInventory : Inventory
{
    private const int MAX_ITEM_TYPES = 3;

    private Worker workerStats;
    public event Action OnInventoryChanged;

    private Dictionary<InventoryObject, int> inputItems = new Dictionary<InventoryObject, int>();
    private Dictionary<InventoryObject, int> outputItems = new Dictionary<InventoryObject, int>();

    public Dictionary<InventoryObject, int> InputItems => inputItems;
    public Dictionary<InventoryObject, int> OutputItems => outputItems;

    private void Awake()
    {
        workerStats = GetComponent<Worker>();
    }

    // --- YARDIMCI METOTLAR ---
    public int GetInputTotal()
    {
        int total = 0;
        foreach (var val in inputItems.Values) total += val;
        return total;
    }

    public int GetOutputTotal()
    {
        int total = 0;
        foreach (var val in outputItems.Values) total += val;
        return total;
    }

    public int GetCapacity()
    {
        return workerStats != null ? workerStats.CarryCapacity : 30;
    }

    // --- GİRDİ (INPUT) İŞLEMLERİ ---
    public int AddToInput(InventoryObject item, int amount)
    {
        int spaceLeft = GetCapacity() - GetInputTotal();
        if (spaceLeft <= 0) return 0;
        if (!inputItems.ContainsKey(item) && inputItems.Count >= MAX_ITEM_TYPES) return 0;

        int toAdd = Mathf.Min(amount, spaceLeft);
        if (inputItems.ContainsKey(item)) inputItems[item] += toAdd;
        else inputItems.Add(item, toAdd);

        OnInventoryChanged?.Invoke();
        return toAdd;
    }

    public int RemoveFromInput(InventoryObject item, int amount)
    {
        if (!inputItems.ContainsKey(item)) return 0;

        int toRemove = Mathf.Min(amount, inputItems[item]);
        inputItems[item] -= toRemove;
        if (inputItems[item] <= 0) inputItems.Remove(item);

        OnInventoryChanged?.Invoke();
        return toRemove;
    }

    // --- ÇIKTI (OUTPUT) İŞLEMLERİ ---
    public int AddToOutput(InventoryObject item, int amount)
    {
        int spaceLeft = GetCapacity() - GetOutputTotal();
        if (spaceLeft <= 0) return 0;
        if (!outputItems.ContainsKey(item) && outputItems.Count >= MAX_ITEM_TYPES) return 0;

        int toAdd = Mathf.Min(amount, spaceLeft);
        if (outputItems.ContainsKey(item)) outputItems[item] += toAdd;
        else outputItems.Add(item, toAdd);

        OnInventoryChanged?.Invoke();
        return toAdd;
    }

    public int RemoveFromOutput(InventoryObject item, int amount)
    {
        if (!outputItems.ContainsKey(item)) return 0;

        int toRemove = Mathf.Min(amount, outputItems[item]);
        outputItems[item] -= toRemove;
        if (outputItems[item] <= 0) outputItems.Remove(item);

        OnInventoryChanged?.Invoke();
        return toRemove;
    }

    // --- LOJİSTİK VE TRANSFER ---
    public int TransferToInputOf(WorkerInventory receiver, InventoryObject item)
    {
        if (receiver == null || item == null) return 0;
        if (!outputItems.ContainsKey(item) || outputItems[item] <= 0) return 0;

        int actuallyAdded = receiver.AddToInput(item, outputItems[item]);

        if (actuallyAdded > 0)
        {
            RemoveFromOutput(item, actuallyAdded);
        }

        return actuallyAdded;
    }



    public bool IsFull() => GetOutputTotal() >= GetCapacity();
    public int GetTotalAmount() => GetOutputTotal();

    public InventoryObject GetFirstItem()
    {
        foreach (var key in outputItems.Keys) return key;
        foreach (var key in inputItems.Keys) return key;
        return null;
    }

    public void TransferAllItemsTo(Inventory targetInventory)
    {
        if (targetInventory is PlayerInventory playerInventory)
        {
            foreach (var pair in outputItems) playerInventory.AddItem(pair.Key, pair.Value);
            foreach (var pair in inputItems) playerInventory.AddItem(pair.Key, pair.Value);

            outputItems.Clear();
            inputItems.Clear();
            OnInventoryChanged?.Invoke();
        }
    }
}