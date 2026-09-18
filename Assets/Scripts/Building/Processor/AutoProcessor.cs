using System;
using System.Collections.Generic;
using UnityEngine;

public class AutoProcessor : Building
{
    [SerializeField] private float productionTime = 2f;
    public float ProductionTime => productionTime;

    [SerializeField] private int storageCapacity = 50;
    public int StorageCapacity => storageCapacity;

    private float timer;
    public float Progress => timer / productionTime;

    private Queue<ItemData> inputQueue = new Queue<ItemData>();
    public Queue<ItemData> InputQueue => inputQueue;


    private ItemData currentItem;
    public ItemData CurrentItem => currentItem;

    public event Action InputQueueChanged;
    public event Action CurrentItemChanged;
    public event Action StorageChanged;

    private Dictionary<ItemData, int> storage = new Dictionary<ItemData, int>();
    public Dictionary<ItemData, int> Storage => storage;

    public string Status
    {
        get
        {
            if (currentItem != null) return "Processing";
            if (inputQueue.Count > 0) return "Starting";
            return "No Item";
        }
    }




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
        if (inventory is PlayerInventory playerInventory)
        {
            foreach (var pair in storage)
            {
                playerInventory.AddItem(pair.Key, pair.Value);
            }

            storage.Clear();
            StorageChanged?.Invoke();
        }
        else if (inventory is WorkerInventory workerInventory)
        {
            Worker collector = workerInventory.GetComponent<Worker>();

            // KORUMA: Sadece taşıma yapan işçi makineden toplayabilir!
            if (collector == null || collector.CurrentState != WorkerState.Transporting)
                return;

            TransportLogic logic = workerInventory.GetComponent<TransportLogic>();
            ItemData transportItem = logic != null ? logic.TransportItem : null;

            if (transportItem == null || !storage.ContainsKey(transportItem) || storage[transportItem] <= 0)
                return;

            int available = storage[transportItem];
            int added = workerInventory.AddToOutput(transportItem, available);
            if (added > 0)
            {
                storage[transportItem] -= added;
                if (storage[transportItem] <= 0)
                {
                    storage.Remove(transportItem);
                }
                StorageChanged?.Invoke();
            }
        }
    }
    
    public void AddInput(Inventory inventory, ItemData item)
    {
        inputQueue.Enqueue(item);

        if (inventory is PlayerInventory playerInventory)
        {
            playerInventory.RemoveItem(item, 1);
        }
        else if (inventory is WorkerInventory workerInventory)
        {
            workerInventory.RemoveFromInput(item, 1);
        }

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

        if (timer >= productionTime)
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