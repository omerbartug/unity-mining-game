using UnityEngine;

// Magazada satisa sunulan tekil urunlerin veri tanimi ScriptableObject'idir.
[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item")]
public class ShopItemSO : ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private ShopCategory category;
    [SerializeField] private int price;

    [Header("Reward")]
    [Tooltip("Satin alindiginda oyuncuya verilecek envanter nesnesi (BuildingData, WorkerData veya ItemData).")]
    [SerializeField] private InventoryObject rewardObject;
    [SerializeField] private int rewardAmount = 1;

    // Disariya erisim icin read-only property'ler:
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public ShopCategory Category => category;
    public int Price => price;
    public InventoryObject RewardObject => rewardObject;
    public int RewardAmount => rewardAmount;
}
