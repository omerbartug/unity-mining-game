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

    public void Open(Worker worker)
    {
        currentWorker = worker;

        // Buton eventlerini temizle ve yeniden bağla (eski tıklamalar üst üste binmesin diye)
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

    public void Close()
    {
        currentWorker = null;
        panel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (currentWorker == null) return;

        nameText.text = $"{currentWorker.Name}";
        statusText.text = "Waiting for assignment (Idle)";
        
        levelText.text = $"({currentWorker.Level} lvl)";

        WorkSpeedText.text = $"Work Speed : {currentWorker.MiningSpeed}";

        MoveSpeedText.text = $"Move Speed : {currentWorker.MovementSpeed}";
    }

    private void OnWorkButtonClicked()
    {
        if (currentWorker == null) return;


        workerManager.SetMoveWorkerMode(true);
        uiManager.CloseAllPanels();
        
    }

    private void OnTransportButtonClicked()
    {
        Debug.Log("Taşıma sistemi daha yapılmadı aga, beklemede kal!");
    }

    private void OnCloseButtonClicked()
    {
        uiManager.CloseAllPanels();
    }

    private void OnUpgradeMiningSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMiningSpeed(500))
        {
            UpdateUI();
        }
    }

    private void OnUpgradeMovementSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMovementSpeed(500))
        {
            UpdateUI();
        }
    }
}