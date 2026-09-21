using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// İşçiler için çift hazneli (Girdi / Çıktı) envanter yönetim bileşenidir.
/// İşçinin çalışma türüne (Madenci, İşleyici, Operatör, Taşıyıcı) göre kapasiteleri dinamik olarak dağıtır.
/// </summary>
public class WorkerInventory : Inventory
{
    private const int MAX_ITEM_TYPES = 3;

    private Worker workerStats;

    public event Action OnInputChanged;
    public event Action OnOutputChanged;

    private Dictionary<InventoryObject, int> inputItems = new Dictionary<InventoryObject, int>();
    private Dictionary<InventoryObject, int> outputItems = new Dictionary<InventoryObject, int>();

    public Dictionary<InventoryObject, int> InputItems => inputItems;
    public Dictionary<InventoryObject, int> OutputItems => outputItems;

    private void Awake()
    {
        workerStats = GetComponent<Worker>();
    }

    // --- ORTAK INVENTORY INTERFACE IMPLEMENTASYONU ---


    // İşçinin mevcut görev tipine göre eşyayı kabul edip edemeyeceğini (uygun haznede yer olup olmadığını) sorgular.
    public override bool CanAccept(InventoryObject item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;
        if (workerStats == null) return CanAddToOutput(item);

        return workerStats.CurrentWorkType switch
        {
            WorkerWorkType.Operating => CanAddToInput(item),
            WorkerWorkType.Processing => (item is ItemData itemData && itemData.processable) 
                ? CanAddToInput(item) 
                : CanAddToOutput(item),

            _ => CanAddToOutput(item) // default
        };
    }


    // İşçinin görev türüne göre eşyayı uygun hazneye (Input veya Output) ekler.
    public override int AddItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;
        if (workerStats == null) return AddToOutput(item, amount);

        return workerStats.CurrentWorkType switch
        {
            WorkerWorkType.Operating => AddToInput(item, amount),
            WorkerWorkType.Processing => (item is ItemData itemData && itemData.processable) 
                ? AddToInput(item, amount) 
                : AddToOutput(item, amount),
            _ => AddToOutput(item, amount)
        };
    }

    /// <summary>
    /// Eşyayı işçinin envanterinden çıkarır.
    /// Operatör işçilerde öncelikle Input'tan, diğer işçilerde ise Output'tan düşer.
    /// </summary>
    public override int RemoveItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        if (workerStats != null && workerStats.CurrentWorkType == WorkerWorkType.Operating)
        {
            if (inputItems.ContainsKey(item)) return RemoveFromInput(item, amount);
            if (outputItems.ContainsKey(item)) return RemoveFromOutput(item, amount);
            return 0;
        }

        if (outputItems.ContainsKey(item)) return RemoveFromOutput(item, amount);
        if (inputItems.ContainsKey(item)) return RemoveFromInput(item, amount);
        return 0;
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

    public int InputCapacity
    {
        get
        {
            if (workerStats == null) return 30;

            int total = workerStats.CarryCapacity;

            return workerStats.CurrentWorkType switch
            {
                WorkerWorkType.Mining => 0,
                WorkerWorkType.Processing => total / 2,
                WorkerWorkType.Operating => total,
                WorkerWorkType.Transporting => 0,
                _ => total / 2 // default
            };
        }
    }

    public int OutputCapacity
    {
        get
        {
            if (workerStats == null) return 30;

            int total = workerStats.CarryCapacity;

            return workerStats.CurrentWorkType switch
            {
                WorkerWorkType.Mining => total,
                WorkerWorkType.Processing => total / 2,
                WorkerWorkType.Operating => 0,
                WorkerWorkType.Transporting => total,
                _ => total / 2 //  default
            };
        }
    }


    // --- KONTROL METOTLARI ---
    public bool CanAddToInput(InventoryObject item)
    {
        if (item == null) return false;
        if (InputCapacity <= 0) return false;
        if (GetInputTotal() >= InputCapacity) return false;
        if (!inputItems.ContainsKey(item) && inputItems.Count >= MAX_ITEM_TYPES) return false;

        if (workerStats != null && workerStats.CurrentWorkType == WorkerWorkType.Processing)
        {
            if (item is not ItemData itemData || !itemData.processable) return false;
        }

        return true;
    }

    public bool CanAddToOutput(InventoryObject item)
    {
        if (item == null) return false;
        if (OutputCapacity <= 0) return false;
        if (GetOutputTotal() >= OutputCapacity) return false;
        if (!outputItems.ContainsKey(item) && outputItems.Count >= MAX_ITEM_TYPES) return false;

        return true;
    }

    // --- GİRDİ (INPUT) İŞLEMLERİ ---
    public int AddToInput(InventoryObject item, int amount)
    {
        if (amount <= 0 || !CanAddToInput(item)) return 0;

        int spaceLeft = InputCapacity - GetInputTotal();
        int toAdd = Mathf.Min(amount, spaceLeft);

        if (inputItems.ContainsKey(item)) inputItems[item] += toAdd;
        else inputItems.Add(item, toAdd);

        OnInputChanged?.Invoke();
        return toAdd;
    }

    public int RemoveFromInput(InventoryObject item, int amount)
    {
        if (!inputItems.ContainsKey(item)) return 0;

        int toRemove = Mathf.Min(amount, inputItems[item]);
        inputItems[item] -= toRemove;

        if (inputItems[item] <= 0) inputItems.Remove(item);

        OnInputChanged?.Invoke();
        return toRemove;
    }

    public void ClearInput()
    {
        if (inputItems.Count == 0) return;
        inputItems.Clear();
        OnInputChanged?.Invoke();
    }

    // --- ÇIKTI (OUTPUT) İŞLEMLERİ ---
    public int AddToOutput(InventoryObject item, int amount)
    {
        if (amount <= 0 || !CanAddToOutput(item)) return 0;

        int spaceLeft = OutputCapacity - GetOutputTotal();
        int toAdd = Mathf.Min(amount, spaceLeft);

        if (outputItems.ContainsKey(item)) outputItems[item] += toAdd;
        else outputItems.Add(item, toAdd);

        OnOutputChanged?.Invoke();
        return toAdd;
    }

    public int RemoveFromOutput(InventoryObject item, int amount)
    {
        if (!outputItems.ContainsKey(item)) return 0;

        int toRemove = Mathf.Min(amount, outputItems[item]);
        outputItems[item] -= toRemove;

        if (outputItems[item] <= 0) outputItems.Remove(item);

        OnOutputChanged?.Invoke();
        return toRemove;
    }

    public void ClearOutput()
    {
        if (outputItems.Count == 0) return;
        outputItems.Clear();
        OnOutputChanged?.Invoke();
    }

    // --- LOJİSTİK VE TRANSFER ---
    public void TransferAllToPlayer(PlayerInventory player)
    {
        if (player == null) return;

        foreach (var pair in inputItems)
            player.AddItem(pair.Key, pair.Value);

        foreach (var pair in outputItems)
            player.AddItem(pair.Key, pair.Value);

        ClearInput();
        ClearOutput();
    }

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

    public int TransferFromOutputOf(WorkerInventory source, InventoryObject item)
    {
        if (source == null || item == null) return 0;
        if (!source.OutputItems.ContainsKey(item) || source.OutputItems[item] <= 0) return 0;

        int available = source.OutputItems[item];
        int added = AddToOutput(item, available);

        if (added > 0)
        {
            source.RemoveFromOutput(item, added);
        }

        return added;
    }

    public bool IsFull()
    {
        if (workerStats == null) return false;

        if (workerStats.CurrentWorkType == WorkerWorkType.Operating)
            return GetInputTotal() >= InputCapacity;

        return GetOutputTotal() >= OutputCapacity;
    }

    public int GetTotalAmount() => GetOutputTotal();

    public InventoryObject GetFirstItem()
    {
        foreach (var key in outputItems.Keys) return key;
        foreach (var key in inputItems.Keys) return key;
        return null;
    }
}