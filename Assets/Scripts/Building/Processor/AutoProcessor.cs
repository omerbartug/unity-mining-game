using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoProcessor : Building
{
    

    private Queue<ItemData> inputQueue = new Queue<ItemData>();
    public Queue<ItemData> InputQueue => inputQueue;


    private ItemData currentItem;
    public ItemData CurrentItem => currentItem;

    public event Action InputQueueChanged;
    public event Action CurrentItemChanged;
    public event Action StorageChanged;

    private Dictionary<ItemData, int> storage = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> Storage => storage;




    private void Update()
    {
        if (currentItem == null)
        {
            TryStartNextItem();
            return;
        }

        ProcessCurrentItem();
    }


    public override void CollectItems(Inventory inventory)
    {
        foreach (var pair in storage)
        {
            inventory.AddItem(pair.Key, pair.Value);
        }

        storage.Clear();
        StorageChanged?.Invoke();
    }
    
    public void AddInput(Inventory inventory, ItemData item)
    {

        inputQueue.Enqueue(item);
        inventory.RemoveItem(item, 1);
        InputQueueChanged?.Invoke();
        
    }

    private void TryStartNextItem()
    {
        if (inputQueue.Count == 0)
        {
            CurrentItemChanged?.Invoke();
            return;
        }

        currentItem = inputQueue.Dequeue();

        InputQueueChanged?.Invoke();
        CurrentItemChanged?.Invoke();

        timer = 0f;
    }

    private void ProcessCurrentItem()
    {
        timer += Time.deltaTime;

        if (timer >= buildingData.productionTime)
        {
            ItemData output = currentItem.rewardItem;

            if (storage.ContainsKey(output))
            {storage[output]++;}

            else
            {storage.Add(output, 1);}

            StorageChanged?.Invoke();

            currentItem = null;
            timer = 0;
        }
    }


}