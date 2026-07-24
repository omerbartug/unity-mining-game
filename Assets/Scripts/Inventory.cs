using System.Collections.Generic;
using UnityEngine;



public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventorySlot[] slots = new InventorySlot[8];

    private void Awake()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot();
            }
        }

    public void AddItem(ItemType item, int amount)
    {
        foreach(var slot in slots){
            if(slot.ItemType == item){
                slot.Amount += amount;
                inventoryUI.Refresh();
                return;
            }
        }
        foreach(var slot in slots){
            if(slot.ItemType == ItemType.None){
                slot.ItemType = item; 
                slot.Amount += amount;
                inventoryUI.Refresh();
                return;
            }
        }
    }

    public void RemoveItem(ItemType item, int amount)
    {
        foreach(var slot in slots){
            if(slot.ItemType == item){
                if(slot.Amount-amount <= 0){
                    Debug.Log("o kadar item yok");
                    return;
                }
                slot.Amount -= amount;
                if(slot.Amount <= 0){
                    slot.ItemType = ItemType.None;
                    slot.Amount = 0;
                }
                inventoryUI.Refresh();
                return;
            }
        }
        Debug.Log("Oyle bi item yok");
    }

    public bool HasItem(ItemType item, int amount)
    {
        foreach(var slot in slots){
            if(slot.ItemType == item && slot.Amount >= amount){
                return true;
            }
        }
        return false;
    }

    public InventorySlot[] GetSlots()
    {
        return slots;
    }
}