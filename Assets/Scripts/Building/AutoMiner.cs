using UnityEngine;

// Maden alanları üzerine kurulan ve zamanla otomatik olarak maden üreten binadır.
public class AutoMiner : Building
{
    [SerializeField] private float productionTime = 2f;
    public float ProductionTime => productionTime;

    [SerializeField] private int storageCapacity = 20;
    public int StorageCapacity => storageCapacity;

    private float timer;
    public float Progress => Mathf.Clamp01(timer / productionTime);

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

    // Altındaki maden alanını (MiningArea) Physics overlap ile tespit eder.
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

    // Maden varsa ve depo dolmamışsa periyodik üretim yapar.
    private void Update()
    {
        if (miningArea == null || storage >= storageCapacity) return;

        timer += Time.deltaTime;

        if (timer >= productionTime)
        {
            storage++;
            timer = 0f;
        }
    }

    // Depodaki madenleri gelen envantere (Oyuncu veya Taşıyıcı İşçi) aktarır.
    public override void CollectItems(Inventory inventory)
    {
        if (storage == 0 || miningArea == null || inventory == null)
            return;

        if (inventory.CanAccept(miningArea.RewardItem, 1))
        {
            int added = inventory.AddItem(miningArea.RewardItem, storage);
            storage -= added;
        }
    }
}