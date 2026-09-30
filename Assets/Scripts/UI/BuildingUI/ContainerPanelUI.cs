using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Kargo konteyneri (CargoContainer) binasının depolanan ürünlerini, kapasitesini ve toplama butonlarını yöneten arayüzdür.
public class ContainerPanelUI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text capacityText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button collectAllButton;

    [Header("Storage Slots (3 Slot)")]
    [SerializeField] private Image[] slotIcons = new Image[3];
    [SerializeField] private TMP_Text[] slotAmounts = new TMP_Text[3];

    private CargoContainer currentContainer;

    // Konteyner panelini açar, event aboneliğini ve buton dinleyicilerini kurar.
    public void Open(CargoContainer container)
    {
        currentContainer = container;

        if (currentContainer != null)
        {
            currentContainer.OnStorageChanged += RefreshUI;
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        if (collectAllButton != null)
        {
            collectAllButton.onClick.RemoveAllListeners();
            collectAllButton.onClick.AddListener(OnCollectAllClicked);
        }

        RefreshUI();
        gameObject.SetActive(true);
    }

    // Konteyner panelini kapatır ve event aboneliğini temizler.
    public void Close()
    {
        if (currentContainer != null)
        {
            currentContainer.OnStorageChanged -= RefreshUI;
        }

        currentContainer = null;
        gameObject.SetActive(false);
    }

    // Depolanan ürünleri, kapasite oranını ve toplama butonu görünürlüğünü günceller.
    private void RefreshUI()
    {
        if (currentContainer == null) return;

        RefreshStorage();
        RefreshCapacityText();

        if (collectAllButton != null)
        {
            collectAllButton.gameObject.SetActive(currentContainer.GetTotalItemCount() > 0);
        }
    }

    // Mevcut doluluk ve maksimum kapasite metnini yeniler.
    private void RefreshCapacityText()
    {
        if (capacityText != null && currentContainer != null)
        {
            capacityText.text = $"{currentContainer.GetTotalItemCount()} / {currentContainer.StorageCapacity}";
        }
    }

    // Depodaki ürünleri slot ikonlarına ve miktarlarına yansıtır.
    private void RefreshStorage()
    {
        if (currentContainer == null || slotIcons == null || slotAmounts == null) return;

        int slotIndex = 0;
        foreach (var pair in currentContainer.StoredItems)
        {
            if (slotIndex >= slotIcons.Length) break;

            if (slotIcons[slotIndex] != null)
            {
                slotIcons[slotIndex].enabled = true;
                slotIcons[slotIndex].sprite = pair.Key.icon;
            }

            if (slotAmounts[slotIndex] != null)
            {
                slotAmounts[slotIndex].gameObject.SetActive(true);
                slotAmounts[slotIndex].text = $"x{pair.Value}";
            }

            slotIndex++;
        }

        ClearEmptySlots(slotIndex);
    }

    // Boş kalan slotların ikonlarını ve metinlerini temizler.
    private void ClearEmptySlots(int startIndex)
    {
        if (slotIcons == null || slotAmounts == null) return;

        for (int i = startIndex; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] != null)
            {
                slotIcons[i].enabled = false;
            }

            if (slotAmounts[i] != null)
            {
                slotAmounts[i].text = "";
                slotAmounts[i].gameObject.SetActive(false);
            }
        }
    }

    // Konteyner kapasite artırma butonuna tıklandığında yükseltmeyi dener.
    private void OnUpgradeClicked()
    {
        if (currentContainer == null) return;

        if (currentContainer.TryUpgradeCapacity())
        {
            Debug.Log($"Kapasite artırıldı! Yeni Kapasite: {currentContainer.StorageCapacity}");
        }
        else
        {
            Debug.Log("Kapasite artırılamadı! (Bakiye yetersiz veya max limite ulaşıldı)");
        }
    }

    // Tümünü Topla butonuna tıklandığında konteynerdeki eşyaları oyuncu envanterine aktarır.
    private void OnCollectAllClicked()
    {
        if (currentContainer == null) return;

        if (PlayerInventory.Instance != null)
        {
            currentContainer.CollectItems(PlayerInventory.Instance);
        }
    }
}
