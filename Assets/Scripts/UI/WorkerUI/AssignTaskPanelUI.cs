using UnityEngine;
using TMPro; // TextMeshPro için gerekli
using UnityEngine.UI; // Butonlar için gerekli

public class AssignTaskPanelUI : MonoBehaviour
{
    [Header("Sistem Referansları")]
    [SerializeField] private GameObject panel;
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] private WorkerUIManager uiManager;

    [Header("UI Metinleri")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI WorkSpeedText;
    [SerializeField] private TextMeshProUGUI MoveSpeedText;


    [Header("UI Butonları")]
    [SerializeField] private Button workButton;
    [SerializeField] private Button transportButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button upgradeMiningSpeedButton;
    [SerializeField] private Button upgradeMovementSpeedButton;

    private Worker currentWorker;

    // Paneli açar, işçi bilgilerini bağlar ve buton dinleyicilerini kurar.
    public void Open(Worker worker)
    {
        currentWorker = worker;

        workButton.onClick.RemoveAllListeners();
        workButton.onClick.AddListener(OnWorkButtonClicked);

        transportButton.onClick.RemoveAllListeners();
        transportButton.onClick.AddListener(OnTransportButtonClicked);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnCloseButtonClicked);

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

        UpdateUI();
        panel.SetActive(true);
    }

    // Paneli kapatır ve seçili işçi referansını sıfırlar.
    public void Close()
    {
        currentWorker = null;
        panel.SetActive(false);
    }

    // İşçinin güncel adı, seviyesi ve hız değerlerini arayüze yansıtır.
    private void UpdateUI()
    {
        if (currentWorker == null) return;

        nameText.text = $"{currentWorker.Name}";
        statusText.text = "Waiting for assignment (Idle)";
        levelText.text = $"({currentWorker.Level} lvl)";
        WorkSpeedText.text = $"Work Speed : {currentWorker.MiningSpeed}";
        MoveSpeedText.text = $"Move Speed : {currentWorker.MovementSpeed}";
    }

    // İşçiyi çalıştırma (hedefe gönderme) modunu başlatır.
    private void OnWorkButtonClicked()
    {
        if (currentWorker == null) return;

        workerManager.SetMoveWorkerMode(true);
        uiManager.CloseAllPanels();
    }

    // İşçi için taşıma kurulum panelini açar.
    private void OnTransportButtonClicked()
    {
        if (currentWorker == null) return;
        uiManager.OpenTransportSetup(currentWorker);
    }

    // Paneli kapatır.
    private void OnCloseButtonClicked()
    {
        uiManager.CloseAllPanels();
    }

    // İşçinin maden kazma hızını yükseltir ve arayüzü günceller.
    private void OnUpgradeMiningSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMiningSpeed())
        {
            UpdateUI();
        }
    }

    // İşçinin hareket hızını yükseltir ve arayüzü günceller.
    private void OnUpgradeMovementSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMovementSpeed())
        {
            UpdateUI();
        }
    }
}