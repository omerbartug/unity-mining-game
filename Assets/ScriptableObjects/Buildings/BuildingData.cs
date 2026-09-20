using UnityEngine;

/// <summary>
/// İnşa edilebilir binaların (AutoMiner, AutoProcessor, CargoContainer) veri ve yerleşim konfigürasyonudur.
/// Envanterde bir InventoryObject gibi taşınabilir ve BuildingManager tarafından sahneye yerleştirilir.
/// </summary>


[CreateAssetMenu(menuName = "Building/Building Data")]

public class BuildingData : InventoryObject
{
    [Header("General")]
    [Tooltip("Binayı satın almak için gereken para miktarı.")]
    public int price;

    [Header("Prefabs")]
    [Tooltip("Dünyaya yerleştirildiğinde sahnede oluşturulacak asıl bina prefabı.")]
    public GameObject buildingPrefab;

    [Tooltip("Yerleştirme modundayken farenin ucunda beliren yarı saydam önizleme (hayalet) prefabı.")]
    public GameObject ghostPrefab;

    [Header("Placement")]
    [Tooltip("Binanın grid üzerinde kapladığı alan (Genişlik x Yükseklik, örn: 2x2).")]
    public Vector2Int size;

    [Tooltip("Binanın üzerine yerleştirilmesini engelleyen katmanlar (Duvarlar, diğer binalar vb.).")]
    public LayerMask placementBlockerLayer;

    [Tooltip("Bina yerleştirilirken aranan zorunlu kaynak katmanı (Örn: AutoMiner için MiningArea katmanı).")]
    public LayerMask fineLayer;


    /// <summary>
    /// Editorde yapilabilecek mantiksal hatalari denetler.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();

        if (price < 0)
        {
            Debug.LogWarning($"[{name}] BuildingData: 'price' negatif olamaz!", this);
        }

        if (size.x <= 0 || size.y <= 0)
        {
            Debug.LogWarning($"[{name}] BuildingData: 'size' genişlik ve yükseklik en az 1 olmalıdır!", this);
        }
    }
}