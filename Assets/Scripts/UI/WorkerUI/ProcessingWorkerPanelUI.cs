using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProcessingWorkerPanelUI : MonoBehaviour
{
    [Header("Genel Bilgiler")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text workSpeedText;
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Input Storage (Sol Sütun)")]
    [SerializeField] private Image[] inputIcons = new Image[3];
    [SerializeField] private TMP_Text[] inputAmounts = new TMP_Text[3];
    [SerializeField] private TMP_Text inputTotalText;

    [Header("Output Storage (Sağ Sütun)")]
    [SerializeField] private Image[] outputIcons = new Image[3];
    [SerializeField] private TMP_Text[] outputAmounts = new TMP_Text[3];
    [SerializeField] private TMP_Text outputTotalText;

    [Header("Geliştirme Butonları")]
    [SerializeField] private Button upgradeMiningSpeedButton;
    [SerializeField] private Button upgradeMovementSpeedButton;

    [Header("Input Etkileşim Butonları (Mobil)")]
    [SerializeField] private Button addInputButton;
    [SerializeField] private Button retrieveInputButton;
    [SerializeField] private Button stopButton;

    [SerializeField] private WorkerUIManager uiManager;
    private Worker currentWorker;
    private WorkerInventory currentInventory;

    // Paneli açar, envanter ve durum eventlerini dinler, buton dinleyicilerini kurar.
    public void Open(Worker worker)
    {
        currentWorker = worker;
        currentInventory = worker != null ? worker.Inventory : null;

        if (currentInventory != null)
        {
            currentInventory.OnInputChanged += RefreshInput;
            currentInventory.OnOutputChanged += RefreshOutput;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged += RefreshStatus;
        }

        PlayerInteraction.OnNearbyWorkerChanged += HandleNearbyWorkerChanged;

        if (upgradeMiningSpeedButton != null)
        {
            upgradeMiningSpeedButton.onClick.RemoveAllListeners();
            upgradeMiningSpeedButton.onClick.AddListener(OnUpgradeMiningSpeedClicked);
        }

        if (upgradeMovementSpeedButton != null)
        {
            upgradeMovementSpeedButton.onClick.RemoveAllListeners();
            upgradeMovementSpeedButton.onClick.AddListener(OnUpgradeMovementSpeedClicked);
        }

        if (addInputButton != null)
        {
            addInputButton.onClick.RemoveAllListeners();
            addInputButton.onClick.AddListener(OnAddInputClicked);
        }

        if (retrieveInputButton != null)
        {
            retrieveInputButton.onClick.RemoveAllListeners();
            retrieveInputButton.onClick.AddListener(OnRetrieveInputClicked);
        }

        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(OnStopButtonClicked);
        }

        RefreshGeneralStats();
        RefreshInput();
        RefreshOutput();
        UpdateButtonStates();

        panel.SetActive(true);
    }

    // Paneli kapatır, tüm event aboneliklerini sonlandırır ve referansları temizler.
    public void Close()
    {
        if (currentInventory != null)
        {
            currentInventory.OnInputChanged -= RefreshInput;
            currentInventory.OnOutputChanged -= RefreshOutput;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged -= RefreshStatus;
        }

        PlayerInteraction.OnNearbyWorkerChanged -= HandleNearbyWorkerChanged;

        currentWorker = null;
        currentInventory = null;
        if (panel != null) panel.SetActive(false);
    }

    // Panel yok edilirken aktif event aboneliklerini temizler.
    private void OnDestroy()
    {
        Close();
    }

    // İşçinin genel istatistik metinlerini günceller.
    private void RefreshGeneralStats()
    {
        if (currentWorker == null) return;

        if (nameText != null) nameText.text = currentWorker.Name;
        RefreshStatus();
        if (levelText != null) levelText.text = $"Level : {currentWorker.Level}";
        if (workSpeedText != null) workSpeedText.text = $"Work Speed : {currentWorker.MiningSpeed}";
        if (moveSpeedText != null) moveSpeedText.text = $"Move Speed : {currentWorker.MovementSpeed}";
    }

    // İşçinin güncel durum metnini günceller.
    private void RefreshStatus()
    {
        if (currentWorker == null) return;
        if (statusText != null) statusText.text = "Status : " + currentWorker.Status;
    }

    // Oyuncunun işçinin etkileşim menzilinde olup olmadığını kontrol eder.
    private bool IsPlayerNearby()
    {
        return PlayerInteraction.Instance != null && PlayerInteraction.Instance.CurrentNearbyWorker == currentWorker;
    }

    // Oyuncu yakındaki işçi menziline girdiğinde veya çıktığında buton durumlarını yeniler.
    private void HandleNearbyWorkerChanged()
    {
        UpdateButtonStates();
    }

    // Oyuncunun yakınlık durumuna göre girdi butonlarının etkileşimini ayarlar.
    private void UpdateButtonStates()
    {
        bool isNearby = IsPlayerNearby();
        if (addInputButton != null) addInputButton.interactable = isNearby;
        if (retrieveInputButton != null) retrieveInputButton.interactable = isNearby;
    }

    // İşçinin girdi haznesindeki ürünleri ve toplam miktarı arayüze yansıtır.
    private void RefreshInput()
    {
        if (currentInventory == null || currentWorker == null) return;

        if (inputTotalText != null)
        {
            inputTotalText.text = $"{currentInventory.GetInputTotal()} / {currentInventory.InputCapacity}";
        }

        int activeCount = DisplayItems(inputIcons, inputAmounts, currentInventory.InputItems);
        ClearEmptySlots(inputIcons, inputAmounts, activeCount);
        RefreshStatus();
    }

    // İşçinin çıktı haznesindeki ürünleri ve toplam miktarı arayüze yansıtır.
    private void RefreshOutput()
    {
        if (currentInventory == null || currentWorker == null) return;

        if (outputTotalText != null)
        {
            outputTotalText.text = $"{currentInventory.GetOutputTotal()} / {currentInventory.OutputCapacity}";
        }

        int activeCount = DisplayItems(outputIcons, outputAmounts, currentInventory.OutputItems);
        ClearEmptySlots(outputIcons, outputAmounts, activeCount);
        RefreshStatus();
    }

    // Belirtilen yuvalara eşya ikonlarını ve adetlerini yerleştirir.
    private int DisplayItems(Image[] icons, TMP_Text[] amounts, Dictionary<InventoryObject, int> items)
    {
        int slotIndex = 0;

        foreach (var pair in items)
        {
            if (slotIndex >= icons.Length) break;

            if (icons[slotIndex] != null)
            {
                icons[slotIndex].enabled = true;
                icons[slotIndex].sprite = pair.Key.icon;
            }

            if (amounts[slotIndex] != null)
            {
                amounts[slotIndex].gameObject.SetActive(true);
                amounts[slotIndex].text = $"x{pair.Value}";
            }

            slotIndex++;
        }

        return slotIndex;
    }

    // Boşta kalan eşya yuvalarını gizler.
    private void ClearEmptySlots(Image[] icons, TMP_Text[] amounts, int startIndex)
    {
        for (int i = startIndex; i < icons.Length; i++)
        {
            if (icons[i] != null)
            {
                icons[i].enabled = false;
            }

            if (amounts[i] != null)
            {
                amounts[i].text = "";
                amounts[i].gameObject.SetActive(false);
            }
        }
    }

    // İşçinin maden kazma hızını yükseltir ve arayüzü yeniler.
    private void OnUpgradeMiningSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMiningSpeed())
        {
            RefreshGeneralStats();
        }
    }

    // İşçinin hareket hızını yükseltir ve arayüzü yeniler.
    private void OnUpgradeMovementSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMovementSpeed())
        {
            RefreshGeneralStats();
        }
    }

    // Oyuncunun elindeki seçili eşyayı işçinin girdi haznesine aktarır.
    private void OnAddInputClicked()
    {
        if (!IsPlayerNearby()) return;
        if (currentInventory == null) return;
        PlayerInventory playerInventory = PlayerInventory.Instance;
        if (playerInventory == null) return;

        InventoryObject selected = playerInventory.GetSelectedItem();
        if (selected != null)
        {
            int amount = playerInventory.GetSelectedSlot().Amount;
            int added = currentInventory.AddToInput(selected, amount);
            if (added > 0)
            {
                playerInventory.RemoveItem(selected, added);
            }
        }
    }

    // İşçinin girdi haznesindeki tüm eşyaları oyuncunun envanterine geri aktarır.
    private void OnRetrieveInputClicked()
    {
        if (!IsPlayerNearby()) return;
        if (currentInventory == null) return;
        PlayerInventory playerInventory = PlayerInventory.Instance;
        if (playerInventory == null) return;

        if (currentInventory.InputItems.Count > 0)
        {
            foreach (var pair in currentInventory.InputItems)
            {
                playerInventory.AddItem(pair.Key, pair.Value);
            }
            currentInventory.ClearInput();
        }
    }

    // İşçiyi çalışmaktan alıkoyup boş (Idle) moda geçirir ve görev atama panelini açar.
    private void OnStopButtonClicked()
    {
        if (currentWorker == null) return;

        Worker worker = currentWorker;
        worker.StopWorking();

        if (uiManager != null)
        {
            uiManager.OpenWorkerUI(worker);
        }
        else
        {
            Close();
        }
    }
}

// Geriye dönük uyumluluk (Unity inspector referansı bozulmasın diye)
public class WorkModePanelUI : ProcessingWorkerPanelUI {}
