using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Çift hazneli (Girdi / Çıktı) modüler envanter bileşenidir.
/// Herhangi bir aktöre veya makineye takılabilir; hazne kapasiteleri dışarıdan yapılandırılır.
/// </summary>
public class WorkerInventory : Inventory
{
    private const int MAX_ITEM_TYPES = 3;

    public event Action OnInputChanged;
    public event Action OnOutputChanged;

    private readonly Dictionary<InventoryObject, int> inputItems = new Dictionary<InventoryObject, int>();
    private readonly Dictionary<InventoryObject, int> outputItems = new Dictionary<InventoryObject, int>();

    public Dictionary<InventoryObject, int> InputItems => inputItems;
    public Dictionary<InventoryObject, int> OutputItems => outputItems;

    [SerializeField] private int inputCapacity = 15;
    [SerializeField] private int outputCapacity = 15;

    public int InputCapacity => inputCapacity;
    public int OutputCapacity => outputCapacity;

    private ItemData transportFilterItem;
    public ItemData TransportFilterItem => transportFilterItem;

    // Envanterin girdi ve çıktı hazne kapasitelerini günceller
    public void SetCapacities(int inputCap, int outputCap)
    {
        inputCapacity = Mathf.Max(0, inputCap);
        outputCapacity = Mathf.Max(0, outputCap);
        OnInputChanged?.Invoke();
        OnOutputChanged?.Invoke();
    }

    // Taşıyıcı modunda sadece seçili eşyanın kabul edilmesini sağlar
    public void SetTransportFilter(ItemData item)
    {
        transportFilterItem = item;
    }

    // Taşıyıcı filtresini kaldırır
    public void ClearTransportFilter()
    {
        transportFilterItem = null;
    }

    // --- ORTAK INVENTORY INTERFACE IMPLEMENTASYONU ---

    public override bool CanAccept(InventoryObject item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        // Taşıyıcı filtresi varsa(isci transportersa) ve gelen eşya seçili eşya değilse reddet
        if (transportFilterItem != null && item != transportFilterItem)
        {
            return false;
        }

        // Girdi haznesi açıksa (Processor sadece işlenmemiş ürün alabilir)
        if (CanAddToInput(item)){return true;}

        // Çıktı haznesi açıksa (Miner, Processor, Transporter)
        if (CanAddToOutput(item)){return true;}

        return false;
    }

    public override int AddItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        if (CanAddToInput(item)){return AddToInput(item, amount);}

        if (CanAddToOutput(item)){return AddToOutput(item, amount);}
       
        return 0;
    }

    public override int RemoveItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        if (outputItems.ContainsKey(item)){return RemoveFromOutput(item, amount);}
        
        if (inputItems.ContainsKey(item)){return RemoveFromInput(item, amount);}
        
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

    // --- KONTROL METOTLARI ---
    public bool CanAddToInput(InventoryObject item)
    {
        if (item == null) return false;
        if (InputCapacity <= 0) return false;
        if (GetInputTotal() >= InputCapacity) return false;
        if (!inputItems.ContainsKey(item) && inputItems.Count >= MAX_ITEM_TYPES) return false;

        // Çıktı haznesi de varsa (Processor), girdiye sadece işlenebilir ürünler girebilir
        if (OutputCapacity > 0 && (item is not ItemData itemData || !itemData.processable))
        {
            return false;
        }

        return true;
    }

    public bool CanAddToOutput(InventoryObject item)
    {
        if (item == null) return false;
        if (transportFilterItem != null && item != transportFilterItem) return false;
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
        if (item == null || amount <= 0) return 0;
        if (!inputItems.TryGetValue(item, out int current)) return 0;

        int toRemove = Mathf.Min(amount, current);
        if (current <= toRemove)
            inputItems.Remove(item);
        else
            inputItems[item] = current - toRemove;

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
        if (item == null || amount <= 0) return 0;
        if (!outputItems.TryGetValue(item, out int current)) return 0;

        int toRemove = Mathf.Min(amount, current);
        if (current <= toRemove)
            outputItems.Remove(item);
        else
            outputItems[item] = current - toRemove;

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
        if (!outputItems.TryGetValue(item, out int available) || available <= 0) return 0;

        int actuallyAdded = receiver.AddToInput(item, available);
        if (actuallyAdded > 0)
        {
            RemoveFromOutput(item, actuallyAdded);
        }

        return actuallyAdded;
    }

    public int TransferFromOutputOf(WorkerInventory source, InventoryObject item)
    {
        if (source == null || item == null) return 0;
        if (!source.OutputItems.TryGetValue(item, out int available) || available <= 0) return 0;

        int added = AddToOutput(item, available);
        if (added > 0)
        {
            source.RemoveFromOutput(item, added);
        }

        return added;
    }

    public bool IsFull()
    {
        bool inputFull = InputCapacity <= 0 || GetInputTotal() >= InputCapacity;
        bool outputFull = OutputCapacity <= 0 || GetOutputTotal() >= OutputCapacity;
        return inputFull && outputFull;
    }
}