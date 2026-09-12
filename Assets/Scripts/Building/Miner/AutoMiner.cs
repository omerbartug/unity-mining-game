using UnityEngine;

public class AutoMiner : Building
{
    [SerializeField] private float productionTime = 2f;
    public float ProductionTime => productionTime;

    [SerializeField] private int storageCapacity = 20;
    public int StorageCapacity => storageCapacity;

    private float timer;
    public float Progress => timer / productionTime;

    private int storage;
    public int StoredItemCount => storage;

    public string Status
    {
        get
        {
            if (miningArea == null) return "No Ore";
            if (storage >= storageCapacity) return "Storage Full";
            return "Mining";
        }
    }

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

        if (timer >= productionTime &&
            storage < storageCapacity)
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

        if (inventory is PlayerInventory playerInventory)
        {
            playerInventory.AddItem(miningArea.RewardItem, storage);
            storage = 0;
        }
    }
 
}