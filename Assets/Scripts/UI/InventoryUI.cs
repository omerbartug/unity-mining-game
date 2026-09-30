using UnityEngine;

// Oyuncu envanterinin tüm slotlarını ekranda oluşturan, güncelleyen ve seçim çerçevesini yöneten bileşendir.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotParent;

    private InventorySlotUI[] slotUIs;
    private int lastSelectedSlotIndex;

    // Envanter referansını doğrular ve slot UI nesnelerini dinamik olarak oluşturur.
    private void Awake()
    {
        if (inventory == null)
            inventory = PlayerInventory.Instance;

        InventorySlot[] slots = inventory != null ? inventory.GetSlots() : null;
        int slotCount = slots != null ? slots.Length : 8;

        slotUIs = new InventorySlotUI[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slotUIs[i] = Instantiate(slotPrefab, slotParent);
            slotUIs[i].Initialize(i, inventory);
        }
    }

    // Envanter değişiklik ve seçim event'lerine abone olur, ilk durum görselini çizer.
    private void Start()
    {
        if (inventory == null)
            inventory = PlayerInventory.Instance;

        if (inventory != null)
        {
            inventory.InventoryChanged += Refresh;
            inventory.SelectedSlotChanged += SetSelectionBorder;
        }

        Refresh();
        SetSelectionBorder();
    }

    // Bellek sızıntılarını önlemek için event aboneliklerini temizler.
    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.InventoryChanged -= Refresh;
            inventory.SelectedSlotChanged -= SetSelectionBorder;
        }
    }

    // Tüm slotların içerik ve miktar görünümlerini yeniler.
    public void Refresh()
    {
        if (inventory == null || slotUIs == null) return;

        InventorySlot[] slots = inventory.GetSlots();
        for (int i = 0; i < slots.Length && i < slotUIs.Length; i++)
        {
            slotUIs[i].Refresh(slots[i]);
        }
    }

    // Seçili slotun etrafındaki çerçeveyi günceller.
    public void SetSelectionBorder()
    {
        if (inventory == null || slotUIs == null) return;

        int index = inventory.GetSelectedSlotIndex();

        slotUIs[lastSelectedSlotIndex].SetSelected(false);
        slotUIs[index].SetSelected(true);

        lastSelectedSlotIndex = index;
    }
}