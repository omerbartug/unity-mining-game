using System;
using UnityEngine;

/// <summary>
/// Oyuncunun 8 yuvalı hotbar envanterini yöneten Singleton bileşenidir.
/// </summary>
public class PlayerInventory : Inventory
{
    public const int SLOT_COUNT = 8;

    [SerializeField] private InventorySlot[] slots = new InventorySlot[SLOT_COUNT];
    private int selectedSlotIndex = 0;

    public event Action SelectedSlotChanged;
    public event Action InventoryChanged;

    public static PlayerInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    // Envanterin bu eşyayı alıp alamayacağını (aynı eşyaya sahip slot veya boş slot var mı) sorgular.
    public override bool CanAccept(InventoryObject item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        foreach (var slot in slots)
        {
            if (slot.CanAccept(item, amount)) return true;
        }
        return false;
    }

    
    // Envantere eşya ekler. Önce aynı eşyanın olduğu slotu, yoksa ilk boş slotu doldurur.
    public override int AddItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        // 1. Önce aynı eşyaya sahip slotu ara
        foreach (var slot in slots)
        {
            if (slot.Data == item)
            {
                int added = slot.AddAmount(amount);
                InventoryChanged?.Invoke();
                return added;
            }
        }

        // 2. Yoksa ilk boş slotu ara
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(item);
                int added = slot.AddAmount(amount);
                InventoryChanged?.Invoke();
                return added;
            }
        }

        Debug.LogWarning($"[PlayerInventory] Envanter dolu! {item.objectName} eklenemedi.");
        return 0;
    }


    // Envanterden eşya eksiltir. Miktar 0'a düşerse InventorySlot kendi kendini temizler.
    public override int RemoveItem(InventoryObject item, int amount)
    {
        if (item == null || amount <= 0) return 0;

        foreach (var slot in slots)
        {
            if (slot.Data == item)
            {
                if (slot.Amount < amount)
                {
                    return 0;
                }

                int removed = slot.RemoveAmount(amount);
                InventoryChanged?.Invoke();
                return removed;
            }
        }

        return 0;
    }

 
    // Belirtilen eşyanın bulunduğu yuvayı tamamen boşaltır.
    public void RemoveAll(InventoryObject item)
    {
        if (item == null) return;

        foreach (var slot in slots)
        {
            if (slot.Data == item)
            {
                slot.Clear();
                InventoryChanged?.Invoke();
                return;
            }
        }
    }


    public InventorySlot[] GetSlots() => slots;
    public InventorySlot GetSelectedSlot() => slots[selectedSlotIndex];
    public InventoryObject GetSelectedItem() => GetSelectedSlot()?.Data;
    public int GetSelectedSlotIndex() => selectedSlotIndex;

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= slots.Length) return;
        selectedSlotIndex = index;
        SelectedSlotChanged?.Invoke();
    }
}