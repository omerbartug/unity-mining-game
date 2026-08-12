using UnityEngine;

public class AutoMiner : Building
{
    

    private int storage;
    public int StoredItemCount => storage;

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
            storage < buildingData.storageCapacity)
        {
            storage++;
            timer = 0f;
        }
    }



    public override void CollectItems(Inventory inventory)
    {
        
        if (storage == 0 || miningArea == null)
        {
            return;
        }

        inventory.AddItem(miningArea.RewardItem, storage);
        storage = 0;
    }
 
}