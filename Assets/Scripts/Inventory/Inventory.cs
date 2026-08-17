using UnityEngine;

public abstract class Inventory : MonoBehaviour
{
    public abstract void AddItem(InventoryObject item, int amount);
    public abstract void RemoveItem(InventoryObject item, int amount);
    public abstract void RemoveAll(InventoryObject item);
    public abstract bool HasItem(InventoryObject item, int amount);
}