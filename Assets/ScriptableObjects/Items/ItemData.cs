using UnityEngine;

/// <summary>
/// Oyundaki toplanabilir ve üretilebilir somut eşyaların (ham madenler, işlenmiş ürünler) veri tanımıdır.
/// Satış değerini, satılabilirlik durumunu ve işlendiğinde neye dönüşeceğini (1:1 tarif) belirler.
/// </summary>


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : InventoryObject
{
    [Header("Economy")]
    [Tooltip("Eşya CargoContainer veya satış noktalarında satıldığında adet başına kazanılacak para miktarı.")]
    public int sellPrice;

    [Tooltip("Bu eşya konteynerlere konup kargo sevkiyatıyla satılabilir mi?")]
    public bool sellable;

    [Header("Processing")]
    [Tooltip("Bu eşya bir ProcessArea veya AutoProcessor içinde işlenebilir mi?")]
    public bool processable;

    [Tooltip("Bu eşya işlendiğinde ortaya çıkacak urun (1:1 dönüşüm).")]
    public ItemData rewardItem;



    /// <summary>
    /// Editörde veri girişi yaparken ekonomik ve mantıksal tutarsızlıkları denetler.
    /// </summary>
    protected  void OnValidate()
    {
        base.OnValidate();

        if (sellable && sellPrice <= 0)
        {
            Debug.LogWarning($"[{name}] ItemData: 'sellable' aktif fakat 'sellPrice' 0 veya daha küçük!", this);
        }

        if (processable && rewardItem == null)
        {
            Debug.LogWarning($"[{name}] ItemData: 'processable' aktif fakat 'rewardItem' atanmamış! İşleme esnasında hata verebilir.", this);
        }

        if (rewardItem == this)
        {
            Debug.LogError($"[{name}] ItemData: 'rewardItem' kendisini referans gösteremez! Sonsuz döngü oluşur.", this);
        }
    }
}