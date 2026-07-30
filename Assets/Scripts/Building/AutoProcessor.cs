using System.Collections.Generic;
using UnityEngine;

public class AutoProcessor : Building, IInteractable
{
    [SerializeField] private ProgressBar progressBar;

    [SerializeField] private LayerMask placementBlockerLayer;
    [SerializeField] private LayerMask okey;

    private Queue<ItemData> inputQueue = new Queue<ItemData>();
    private Queue<ItemData> outputQueue = new Queue<ItemData>();

    private ItemData currentItem;

    private float processTimer;
    private float interactTimer;

    [SerializeField] private float interactTime = 2f;

    private void Update()
    {
        if (currentItem == null)
        {
            if (inputQueue.Count > 0)
            {
                currentItem = inputQueue.Dequeue();
                processTimer = 0;
            }

            return;
        }

        processTimer += Time.deltaTime;

        if (processTimer >= buildingData.productionTime)
        {
            outputQueue.Enqueue(currentItem.rewardItem);

            currentItem = null;
            processTimer = 0;
        }
    }

    public void Interact(Inventory inventory)
    {
        InventoryObject selected = inventory.GetSelectedItem();

        if (!(selected is ItemData item))
            return;

        if (!item.processable)
            return;

        interactTimer += Time.deltaTime;
        progressBar.SetProgress(interactTimer, interactTime);

        if (interactTimer < interactTime)
            return;

        interactTimer = 0;
        progressBar.ResetProgress();

        int amount = inventory.GetSelectedSlot().Amount;

        AddToQueue(item, amount);
        inventory.RemoveAll(item);
    }

    public void ResetInteract()
    {
        interactTimer = 0;
        progressBar.ResetProgress();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory == null)
            return;

        while (outputQueue.Count > 0)
        {
            inventory.AddItem(outputQueue.Dequeue(), 1);
        }
    }

    private void AddToQueue(ItemData item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            inputQueue.Enqueue(item);
        }
    }

    public override bool CheckPlacement(Vector2 position, Vector2 size)
    {
       return false;
    }
}