using System.Collections.Generic;
using UnityEngine;
using System;
public class AutoProcessor : Building
{
    

    private Queue<ItemData> inputQueue = new Queue<ItemData>();
    public Queue<ItemData> InputQueue => inputQueue;


    private ItemData currentItem;
    public ItemData CurrentItem => currentItem;

    public event Action QueueChanged;
    public event Action CurrentItemChanged;
    public event Action OutputChanged;

    private Dictionary<ItemData, int> storage = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> Storage => storage;




    private void Update()
    {
        if (currentItem == null)
        {
            if (inputQueue.Count > 0)
            {
                currentItem = inputQueue.Dequeue();
                QueueChanged?.Invoke();
                CurrentItemChanged?.Invoke();
                timer = 0;
            }

            return;
        }

        ItemData output = currentItem.rewardItem;
        timer += Time.deltaTime;

        if (timer >= buildingData.productionTime)
        {

            if (storage.ContainsKey(output)){storage[output]++;}
            else{storage.Add(output, 1);}
            OutputChanged?.Invoke();

            currentItem = null;
            CurrentItemChanged?.Invoke();

            timer = 0;
        }
    }

    private void AddToQueue(ItemData item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            inputQueue.Enqueue(item);
        }
        QueueChanged?.Invoke();
    }

    public override void CollectItems(Inventory inventory)
    {
        foreach (var pair in storage)
        {
            inventory.AddItem(pair.Key, pair.Value);
        }

        storage.Clear();
        OutputChanged?.Invoke();
    }
    
    public void AddInput(Inventory inventory, ItemData item)
    {
        
        if(!item.processable)
            return;
         
        if (inputQueue.Count >= buildingData.storageCapacity)
            return;

        inputQueue.Enqueue(item);
        inventory.RemoveItem(item, 1);
        QueueChanged?.Invoke();
    }
}