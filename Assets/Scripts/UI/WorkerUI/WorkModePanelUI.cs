using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan
using UnityEngine.UI;

public class WorkModePanelUI: MonoBehaviour
{
    [Header("UI Elemanları")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image iconImage;

    [Header("Geliştirme Butonları")]
    [SerializeField] private Button upgradeMiningSpeedButton;
    [SerializeField] private Button upgradeMovementSpeedButton;

    [SerializeField] WorkerManager workerManager;
    private Worker currentWorker;
    private WorkerInventory currentInventory;
    

    

    public void Open(Worker worker)
    {
        currentWorker = worker;
        currentInventory = worker.Inventory;

        currentInventory.OnInventoryChanged += UpdateUI;

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
        if (currentInventory != null)
        {
            currentInventory.OnInventoryChanged -= UpdateUI;
        }

        currentWorker = null;
        currentInventory = null;
        panel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (currentWorker == null) return;

        nameText.text = currentWorker.Name;
        levelText.text = $"Level: {currentWorker.Level}"; 
        speedText.text = $"Mining Speed: {currentWorker.MiningSpeed}";
        capacityText.text = $"{currentInventory.GetTotalAmount()} / {currentWorker.CarryCapacity}";
        
        if (statusText != null)
        {
            statusText.text = "Status : " + currentWorker.Status;
        }
        
        InventoryObject carriedItem = currentInventory.GetFirstItem();
        if (carriedItem is ItemData itemData)
        {
            iconImage.sprite = itemData.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
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