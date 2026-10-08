using System.Collections.Generic;
using UnityEngine;

// Magazada satisa sunulan tum urunlerin toplu listesini tutan katalog ScriptableObject'idir.
[CreateAssetMenu(fileName = "ShopCatalogue", menuName = "Shop/Catalogue")]
public class ShopCatalogueSO : ScriptableObject
{
    [SerializeField] private List<ShopItemSO> items = new List<ShopItemSO>();

    // Tum urun listesini disariya salt okunur (read-only) koleksiyon olarak sunar.
    public IReadOnlyList<ShopItemSO> Items => items;

    // Belirtilen kategoriye ait urunleri filtreleyip liste olarak doner.
    public List<ShopItemSO> GetItemsByCategory(ShopCategory category)
    {

        List<ShopItemSO> filtered = new List<ShopItemSO>();

        for(int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].Category == category)
            {
                filtered.Add(items[i]);
            }
        }

        return filtered;
    }
}
