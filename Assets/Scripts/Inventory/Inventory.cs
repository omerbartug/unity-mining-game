using System;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots = new InventorySlot[8];

  
    private int selectedSlotIndex = 0;
    public event Action SelectedSlotChanged;
    public event Action InventoryChanged;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
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
                InventoryChanged?.Invoke();
                return;
            }
        }
        foreach(var slot in slots){
            if(slot.Data == null){
                slot.SetItem(item);
                slot.AddAmount(amount);
                InventoryChanged?.Invoke();
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
 
                    slot.Clear();
                }

                InventoryChanged?.Invoke();
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

                InventoryChanged?.Invoke();
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
   public InventorySlot GetSelectedSlot()
    {
        return slots[selectedSlotIndex];
    }
    public InventoryObject GetSelectedItem(){
        return GetSelectedSlot().Data;
    }
    public int GetSelectedSlotIndex()
    {
        return selectedSlotIndex;
    }
    
    public void SelectSlot(int index)
    {
        if (index == selectedSlotIndex)
            return;

        selectedSlotIndex = index;
        SelectedSlotChanged?.Invoke();
    }


}