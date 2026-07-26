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

    public void AddItem(ItemData item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Item == item){
                slot.AddAmount(amount);
                inventoryUI.Refresh();
                return;
            }
        }
        foreach(var slot in slots){
            if(slot.Item == null){
                slot.SetItem(item);
                slot.AddAmount(amount);
                inventoryUI.Refresh();
                return;
            }
        }
    }

    public void RemoveItem(ItemData item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Item == item){
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
    }

    public void RemoveAll(ItemData item){
        foreach(var slot in slots){
            if(slot.Item == item){
                slot.Clear();
                inventoryUI.Refresh();
                return;
            }
        }
        Debug.Log("bilinemeyen hata");
    }
    public bool HasItem(ItemData item, int amount)
    {
        foreach(var slot in slots){
            if(slot.Item == item && slot.Amount >= amount){
                return true;
            }
        }
        return false;
    }

    public InventorySlot[] GetSlots()
    {
        return slots;
    }

    public InventorySlot getSelectedSlot(){
        return slots[inventoryUI.getSelectedSlotIndex()];
    }
    public ItemData getSelectedItem(){
        return getSelectedSlot().Item;
    }
}