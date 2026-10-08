using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Magaza ana penceresini, kategori sekmelerini ve vitrin slotlarinin uretimini yonetir.
public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject openShopButton;
    [SerializeField] private Transform container;
    [SerializeField] private ShopSlotUI slotPrefab;

    [Header("Scroll References")]
    [SerializeField] private ScrollRect scrollRect;

    private List<ShopSlotUI> activeSlots = new List<ShopSlotUI>();
    private ShopCategory currentCategory = ShopCategory.Buildings;

    private GameObject TargetPanel => shopPanel != null ? shopPanel : gameObject;

    private void Awake()
    {
        // Panel kapali olsa bile P tusunu dinleyebilmek icin Awake'te abone ol
        PlayerInputManager.OnToggleShop += Toggle;
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnToggleShop -= Toggle;
    }

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged += HandleMoneyChanged;

        if (ShopManager.Instance != null)
            ShopManager.Instance.OnPurchaseSuccess += HandlePurchaseSuccess;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.InventoryChanged += RefreshAllSlots;
    }

    private void OnDisable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged -= HandleMoneyChanged;

        if (ShopManager.Instance != null)
            ShopManager.Instance.OnPurchaseSuccess -= HandlePurchaseSuccess;

        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.InventoryChanged -= RefreshAllSlots;
    }

    private void Start()
    {
        bool isPanelOpen = TargetPanel.activeInHierarchy;

        // Oyun basladiginda ufak shop butonunun gorunurlugunu panelin tam tersi yap
        if (openShopButton != null)
        {
            openShopButton.SetActive(!isPanelOpen);
        }

        // Panel oyun basladiginda sahnede zaten aciksa urunleri hemen yukle
        if (isPanelOpen)
        {
            ShowCategory(currentCategory);
        }
    }

    // Magaza aciksa kapatir, kapaliysa acar (P tusu ve HUD Shop butonu icin).
    public void Toggle()
    {
        if (TargetPanel.activeSelf)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    // Magaza panelini acar, ufak shop butonunu gizler ve varsayilan kategoriyi listeler.
    public void Open()
    {
        TargetPanel.SetActive(true);

        if (openShopButton != null)
        {
            openShopButton.SetActive(false);
        }

        ShowCategory(currentCategory);
    }

    // Magaza panelini kapatir ve ufak shop butonunu tekrar gosterir (Carpi X butonu icin).
    public void Close()
    {
        TargetPanel.SetActive(false);

        if (openShopButton != null)
        {
            openShopButton.SetActive(true);
        }
    }

    // Belirtilen kategoriye ait urunleri container icerisinde olusturur.
    public void ShowCategory(ShopCategory category)
    {
        currentCategory = category;

        // Onceki kategoriden kalan eski kartlari temizle
        ClearSlots();

        if (ShopManager.Instance == null || ShopManager.Instance.Catalogue == null || container == null || slotPrefab == null)
        {
            return;
        }

        var items = ShopManager.Instance.Catalogue.GetItemsByCategory(category);
        if (items == null) return;

        foreach (var item in items)
        {
            ShopSlotUI slot = Instantiate(slotPrefab, container);
            slot.Setup(item);
            activeSlots.Add(slot);
        }

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f; // Scroll'u en yukariya sifirla
        }
    }

    // Secili kategorideki tum kartlarin buton durumlarini (yeterli para, envanter yeri) gunceller.
    public void RefreshAllSlots()
    {
        foreach (var slot in activeSlots)
        {
            if (slot != null)
            {
                slot.RefreshButtonState();
            }
        }
    }

    // Sahnedeki eski slot nesnelerini temizler.
    private void ClearSlots()
    {
        foreach (var slot in activeSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        activeSlots.Clear();
    }

    // Para degistiginde tetiklenen event dinleyicisi.
    private void HandleMoneyChanged(int newMoney)
    {
        RefreshAllSlots();
    }

    // Satin alma basarili oldugunda tetiklenen event dinleyicisi.
    private void HandlePurchaseSuccess(ShopItemSO item)
    {
        RefreshAllSlots();
    }

    // Kategori butonlarının OnClick'ine Unity editorunden dogrudan bu metodlari baglayabilirsin:
    public void OnBuildingsTabClicked()
    {
        ShowCategory(ShopCategory.Buildings);
    }

    public void OnWorkersTabClicked()
    {
        ShowCategory(ShopCategory.Workers);
    }

    public void OnItemsTabClicked()
    {
        ShowCategory(ShopCategory.Items);
    }
}
