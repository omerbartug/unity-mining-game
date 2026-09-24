using UnityEngine;

// Oyundaki tüm binaların (Maden, İşlemci, Konteyner) türediği temel abstract sınıftır.
public abstract class Building : MonoBehaviour, IItemSource
{
    [SerializeField] protected BuildingData buildingData;
   
    // Binanın konfigürasyon ve yerleşim verilerini tutan ScriptableObject referansı.
    public BuildingData Data => buildingData;

    // Binadan eşya toplanmasını sağlayan soyut metot (Alt sınıflar kendine göre uygular).
    public abstract void CollectItems(Inventory inventory);
}