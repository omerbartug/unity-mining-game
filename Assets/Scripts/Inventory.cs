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

    public void AddItem(InventoryObject item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Data == item){
                slot.AddAmount(amount);
                inventoryUI.Refresh();
                return;
            }
        }
        foreach(var slot in slots){
            if(slot.Data == null){
                slot.SetItem(item);
                slot.AddAmount(amount);
                inventoryUI.Refresh();
                return;
            }
        }
    }

    public void RemoveItem(InventoryObject item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Data == item){
                if(slot.Amount-amount < 0){
                    Debug.Log("o kadar item yok");
                    return;
                }
                slot.RemoveAmount(amount);
                if(slot.Amount <= 0){
                    Debug.Log("Item Siliniyor");
                    slot.Clear();
                }
                inventoryUI.Refresh();
                return;
            }
        }
        Debug.Log("Oyle bi item yok");
        return;
    }

    public void RemoveAll(InventoryObject item){
        foreach(var slot in slots){
            if(slot.Data == item){
                slot.Clear();
                inventoryUI.Refresh();
                return;
            }
        }
        Debug.Log("O item yok");
        return;
    }
    public bool HasItem(InventoryObject item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Data == item && slot.Amount >= amount){
                return true;
            }
        }
        return false;
    }

    public InventorySlot[] GetSlots()
    {
        return slots;
    }

    public InventorySlot GetSelectedSlot(){
        return slots[inventoryUI.getSelectedSlotIndex()];
    }
    public InventoryObject GetSelectedItem(){
        return GetSelectedSlot().Data;
    }
}