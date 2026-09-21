
using UnityEngine;

/// <summary>
/// Envanterdeki tek bir yuvayı (slot) temsil eden veri sınıfıdır.
/// Bir InventoryObject referansı ve miktarını tutar.
/// Kendi sınırlarını (negatiflik koruması, 0 olunca otomatik temizleme) kendisi yönetir.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public InventoryObject Data { get; private set; }
    public int Amount { get; private set; }


    // Slotun boş olup olmadığını belirtir (Data yoksa veya miktar 0 ise boştur).
    public bool IsEmpty => Data == null || Amount <= 0;


    // Bu slota belirtilen eşyanın eklenip eklenemeyeceğini sorgular.
    public bool CanAccept(InventoryObject item, int addAmount = 1)
    {
        if (item == null || addAmount <= 0) return false;
        if (IsEmpty) return true;
        return Data == item;
    }


    // Slota eşya türü atar.
    public void SetItem(InventoryObject data)
    {
        Data = data;
    }

    
    // Slottaki eşya miktarını artırır. Gerçekte eklenen miktarı döner.
    public int AddAmount(int amount)
    {
        if (amount <= 0) return 0;
        Amount += amount;
        return amount;
    }

    /// <summary>
    /// Slottaki eşya miktarını azaltır. Negatife düşmeyi engeller.
    /// Miktar 0'a ulaştığında slot otomatik olarak kendini temizler (Clear).
    /// Gerçekte eksiltilen miktarı döner.
    /// </summary>
    public int RemoveAmount(int amount)
    {
        if (amount <= 0 || IsEmpty) return 0;

        int actualRemoved = Mathf.Min(amount, Amount);
        Amount -= actualRemoved;

        if (Amount <= 0)
        {
            Clear();
        }

        return actualRemoved;
    }


    // Slotu tamamen sıfırlar ve boşaltır.
    public void Clear()
    {
        Data = null;
        Amount = 0;
    }
}