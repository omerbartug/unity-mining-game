using UnityEngine;

/// <summary>
/// Oyundaki tüm envanter yapılarının (PlayerInventory, WorkerInventory) ortak temel sınıfıdır.
/// Tüm sistemlerin (Madencilik, Binalar, Lojistik) aktör tipini bilmek zorunda kalmadan (downcast yapmadan)
/// eşya alıp verebilmesini sağlayan ortak kontratı belirler.
/// </summary>
public abstract class Inventory : MonoBehaviour
{
    /// <summary>
    /// Envanterin bu eşyadan belirtilen miktarda kabul edip edemeyeceğini (kapasite/yer kontrolü) sorgular.
    /// </summary>
    public abstract bool CanAccept(InventoryObject item, int amount = 1);

    /// <summary>
    /// Envantere eşya ekler. Gerçekte eklenen miktarı döner.
    /// </summary>
    public abstract int AddItem(InventoryObject item, int amount);

    /// <summary>
    /// Envanterden belirtilen miktarda eşya çıkarır. Gerçekte çıkarılan miktarı döner.
    /// </summary>
    public abstract int RemoveItem(InventoryObject item, int amount);

}


// daha genis kapsamli biur hale getirilmeli downcasting preoblemleri cozulmeli.