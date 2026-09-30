using UnityEngine;

/// <summary>
/// Haritaya yerleştirilebilir işçilerin veri ve yerleşim konfigürasyonudur.
/// Envanterde bir InventoryObject gibi taşınabilir ve WorkerManager tarafından sahneye yerleştirilir.
/// </summary>
[CreateAssetMenu(menuName = "Worker/Worker Data")]
public class WorkerData : InventoryObject
{
    [Header("General")]
    [Tooltip("İşçiyi satın almak için gereken para miktarı.")]
    public int price;

    [Header("Prefabs")]
    [Tooltip("Dünyaya yerleştirildiğinde sahnede oluşturulacak asıl işçi prefabı.")]
    public GameObject workerPrefab;

    [Tooltip("Yerleştirme modundayken farenin ucunda beliren yarı saydam önizleme (hayalet) prefabı.")]
    public GameObject ghostPrefab;

    [Header("Placement")]
    [Tooltip("İşçinin grid üzerinde kapladığı alan (Genişlik x Yükseklik, varsayılan 1x1).")]
    public Vector2Int size = Vector2Int.one;

    [Tooltip("İşçinin üzerine yerleştirilmesini engelleyen katmanlar (Duvarlar, binalar vb.).")]
    public LayerMask placementBlockerLayer;

    [Tooltip("İşçi yerleştirilirken aranan zorunlu zemin/alan katmanı (Boş bırakılırsa her engelsiz yere yerleştirilebilir).")]
    public LayerMask fineLayer;

    // Editörde yapılabilecek mantıksal hataları denetler.
    protected override void OnValidate()
    {
        base.OnValidate();

        if (price < 0)
        {
            Debug.LogWarning($"[{name}] WorkerData: 'price' negatif olamaz!", this);
        }

        if (size.x <= 0 || size.y <= 0)
        {
            Debug.LogWarning($"[{name}] WorkerData: 'size' genişlik ve yükseklik en az 1 olmalıdır!", this);
        }
    }
}
