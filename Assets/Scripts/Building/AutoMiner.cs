using UnityEngine;
using System.Collections.Generic;

public class AutoMiner : Building
{
    

    private int Storage;
    public int StoredItemCount => Storage;

    private MiningArea miningArea;

    private void Awake()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();

        Collider2D ore = Physics2D.OverlapBox(
            box.bounds.center,
            box.bounds.size,
            0f,
            buildingData.fineLayer
        );

        if (ore != null)
        {
            miningArea = ore.GetComponent<MiningArea>();
        }
    }

    private void Update()
    {
        if(miningArea == null) return;
        timer += Time.deltaTime;

        if (timer >= buildingData.productionTime &&
            Storage < buildingData.storageCapacity)
        {
            Storage++;
            timer = 0f;
        }
    }



    public override void CollectItems(Inventory inventory)
    {
        
        if (Storage == 0 || miningArea == null)
        {
            return;
        }

        inventory.AddItem(miningArea.RewardItem, Storage);
        Storage = 0;
    }


    
}