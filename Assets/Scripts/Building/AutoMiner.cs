using UnityEngine;
using System.Collections.Generic;

public class AutoMiner : Building
{
    private float timer;
    [SerializeField] private LayerMask placementBlockerLayer;
    [SerializeField] private LayerMask oreLayer;
    private Queue<ItemData> storageQueue = new Queue<ItemData>();

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= buildingData.productionTime &&
            storageQueue.Count < buildingData.storageCapacity)
        {
            storageQueue.Enqueue(buildingData.outputItem);
            timer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();
        if (inventory == null)
            return;

        int amount = storageQueue.Count;
        if (amount == 0)
            return;

        storageQueue.Clear();
        inventory.AddItem(buildingData.outputItem, amount);

        Debug.Log($"Collected {amount}");
    }

    public override bool CheckPlacement(Vector2 position, Vector2 size)
    {
        
        Collider2D blocker = Physics2D.OverlapBox(position, size, 0, placementBlockerLayer);
        Collider2D ore = Physics2D.OverlapBox(position, size, 0, oreLayer);

       
        return blocker == null && ore != null;
    }
    public override void OnSelected()
    {
        Debug.Log(storageQueue.Count);
    }
}