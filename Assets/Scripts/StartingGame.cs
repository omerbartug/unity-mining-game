using System;
using System.Collections.Generic;
using UnityEngine;

// Oyun başlangıcında oyuncu envanterine başlangıç/test eşyalarını veren yardımcı bileşendir.
public class StartingGame : MonoBehaviour
{
    [Serializable]
    public struct StartingItem
    {
        [Tooltip("Verilecek eşya, bina veya işçi verisi.")]
        public InventoryObject item;

        [Tooltip("Verilecek adet.")]
        public int amount;
    }

    [Header("Settings")]
    [SerializeField] private int targetFrameRate = 120;

    [Header("Starting / Debug Items")]
    [SerializeField] private List<StartingItem> startingItems = new List<StartingItem>();

    // Hedef kare hızını ayarlar.
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    // Oyuncu envanteri hazır olduğunda başlangıç eşyalarını dağıtır.
    private void Start()
    {
        GiveStartingItems();
    }

    // Listede tanımlı olan tüm başlangıç eşyalarını oyuncu envanterine ekler.
    public void GiveStartingItems()
    {
        PlayerInventory inventory = PlayerInventory.Instance;
        if (inventory == null) return;

        foreach (var entry in startingItems)
        {
            if (entry.item != null && entry.amount > 0)
            {
                inventory.AddItem(entry.item, entry.amount);
            }
        }
    }
}
