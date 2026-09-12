using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ContainerPanelUI : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text capacityText; // "57 / 60"
    [SerializeField] private Button upgradeButton;  // "+" Butonu
    [SerializeField] private Button collectAllButton; // "Collect All" Butonu

    [Header("Storage Slots (3 Slot)")]
    [SerializeField] private Image[] slotIcons = new Image[3];
    [SerializeField] private TMP_Text[] slotAmounts = new TMP_Text[3];

    private CargoContainer currentContainer;

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

    public void Close()
    {
        if (currentContainer != null)
        {
            currentContainer.OnStorageChanged -= RefreshUI;
        }

        currentContainer = null;
        gameObject.SetActive(false);
    }

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

    private void RefreshCapacityText()
    {
        if (capacityText != null && currentContainer != null)
        {
            capacityText.text = $"{currentContainer.GetTotalItemCount()} / {currentContainer.StorageCapacity}";
        }
    }

    private void RefreshStorage()
    {
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

    private void ClearEmptySlots(int startIndex)
    {
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

    private void OnUpgradeClicked()
    {
        if (currentContainer == null) return;

        // 500 altın, +10 kapasite, max 60 limit
        if (currentContainer.TryUpgradeCapacity(500, 10, 60))
        {
            Debug.Log($"Kapasite artırıldı! Yeni Kapasite: {currentContainer.StorageCapacity}");
        }
        else
        {
            Debug.Log("Kapasite artırılamadı! (Bakiye yetersiz veya max limite ulaşıldı)");
        }
    }

    private void OnCollectAllClicked()
    {
        if (currentContainer == null) return;

        if (PlayerInventory.Instance != null)
        {
            currentContainer.CollectItems(PlayerInventory.Instance);
        }
    }
}
