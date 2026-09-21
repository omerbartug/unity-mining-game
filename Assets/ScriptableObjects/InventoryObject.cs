using UnityEngine;

/// <summary>
/// Envanterde yer alabilecek tüm nesnelerin (eşyalar ve binalar) temel sınıfıdır.
/// Sahnedeki envanter nesneleri bu tekil ScriptableObject referansını paylaşır.
/// </summary>
public abstract class InventoryObject : ScriptableObject
{
    [Header("Item Info")]
    [Tooltip("Nesnenin kullanıcı arayüzünde ve bildirimlerde görünecek adı.")]
    public string objectName;

    [Tooltip("Nesnenin envanter slotlarında ve panellerde görünecek simgesi.")]
    public Sprite icon;

    /// <summary>
    /// Editörde veri girişi yaparken eksik alanları denetler ve uyarı verir.
    /// </summary>
    protected virtual void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            Debug.LogWarning($"[{name}] InventoryObject: 'objectName' alanı boş bırakılmış!", this);
        }

        if (icon == null)
        {
            Debug.LogWarning($"[{name}] InventoryObject: 'icon' sprite'ı atanmamış!", this);
        }
    }
}
