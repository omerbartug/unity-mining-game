using System;
using System.Collections.Generic;
using UnityEngine;

// Ham maddeleri girdi kuyruğuna alıp sırayla işleyen ve mamul ürün üreten otomatik tesistir.
public class AutoProcessor : Building
{
    [SerializeField] private float productionTime = 2f;
    public float ProductionTime => productionTime;

    [SerializeField] private int storageCapacity = 50;
    public int StorageCapacity => storageCapacity;

    private float timer;
    public float Progress => Mathf.Clamp01(timer / productionTime);

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

    // Depodaki işlenmiş ürünleri alabilen envantere (Oyuncu veya Taşıyıcı İşçi) aktarır.
    public override void CollectItems(Inventory inventory)
    {
        if (storage.Count == 0 || inventory == null)
            return;

        // Döngü sırasında sözlükten silme yapabilmek için kopyasını geziyoruz
        var itemsToCollect = new List<KeyValuePair<ItemData, int>>(storage);
        bool changed = false;

        foreach (var pair in itemsToCollect)
        {
            if (inventory.CanAccept(pair.Key))
            {
                int added = inventory.AddItem(pair.Key, pair.Value);
                if (added > 0)
                {
                    storage[pair.Key] -= added;
                    if (storage[pair.Key] <= 0)
                    {
                        storage.Remove(pair.Key);
                    }
                    changed = true;
                }
            }
        }

        if (changed)
        {
            StorageChanged?.Invoke();
        }
    }

    // İşlemci kuyruğuna yeni bir ham madde ekler.
    public void AddInput(ItemData item, int amount)
    {
        if (item == null) return;
        for(int i = 0; i < amount; i++)
        {
            inputQueue.Enqueue(item);
        }
        InputQueueChanged?.Invoke();
    }

    // Kuyrukta bekleyen sıradaki ham maddeyi işlemeye alır.
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

    // Mevcut ham maddenin üretim süresini ilerletir ve tamamlanınca mamul depoya atar.
    private void ProcessCurrentItem()
    {
        timer += Time.deltaTime;

        if (timer >= productionTime)
        {
            ItemData output = currentItem.rewardItem;

            if (output != null)
            {
                if (storage.ContainsKey(output))
                    storage[output]++;
                else
                    storage.Add(output, 1);

                StorageChanged?.Invoke();
            }

            currentItem = null;
            timer = 0f;
        }
    }
}