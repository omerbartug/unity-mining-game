using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();

    public void AddItem(ItemType item, int amount)
    {
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        }
        else
        {
            items.Add(item, amount);
        }
        Debug.Log($"Item: {item}, Value: {items[item]}");
    }

    public void RemoveItem(ItemType item, int amount)
    {
        if(!items.ContainsKey(item)){
            return;
        }
        items[item] -= amount;

        if(items[item] <= 0){
            items.Remove(item);
        }
    }

    public void GetInventory(){
        foreach(KeyValuePair<ItemType, int> component in items){
            Debug.Log($"Item : {component.Key}, Amount : {component.Value}");
        }
    }
}