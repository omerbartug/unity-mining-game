using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TransportingWorkerPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Genel Bilgiler")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Taşıma Bilgisi & Eşya")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text cargoTotalText;

    [Header("Butonlar")]
    [SerializeField] private Button stopButton;
    [SerializeField] private Button upgradeMoveSpeedButton;

    [Header("Referanslar")]
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] private WorkerUIManager uiManager;

    private Worker currentWorker;
    private WorkerInventory currentInventory;
    private TransportLogic currentTransportLogic;
    private ItemData selectedItem;
    private bool isSetupMode = false;

    public void OpenForSetup(Worker worker)
    {
        Close();

        currentWorker = worker;
        currentInventory = worker != null ? worker.Inventory : null;
        currentTransportLogic = worker != null ? worker.GetComponent<TransportLogic>() : null;
        selectedItem = null;
        isSetupMode = true;

        RefreshGeneralStats();

        if (statusText != null) statusText.text = "Envanterden taşınacak eşyayı seçin!";
        if (itemNameText != null) itemNameText.text = "Bir İtem Seç...";
        if (itemIcon != null) itemIcon.enabled = false;
        if (cargoTotalText != null) cargoTotalText.text = $"- / {(currentWorker != null ? currentWorker.CarryCapacity : 30)}";

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.SelectedSlotChanged += OnInventorySlotSelected;
        }

        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(OnCancelSetupClicked);
        }

        if (upgradeMoveSpeedButton != null)
        {
            upgradeMoveSpeedButton.onClick.RemoveAllListeners();
            upgradeMoveSpeedButton.onClick.AddListener(OnUpgradeMoveSpeedClicked);
        }

        panel.SetActive(true);
    }

    public void Open(Worker worker)
    {
        Close();

        currentWorker = worker;
        currentInventory = worker != null ? worker.Inventory : null;
        currentTransportLogic = worker != null ? worker.GetComponent<TransportLogic>() : null;
        isSetupMode = false;

        if (currentInventory != null)
        {
            currentInventory.OnOutputChanged += RefreshCargo;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged += RefreshStatus;
        }

        if (stopButton != null)
        {
            stopButton.onClick.RemoveAllListeners();
            stopButton.onClick.AddListener(OnStopButtonClicked);
        }

        if (upgradeMoveSpeedButton != null)
        {
            upgradeMoveSpeedButton.onClick.RemoveAllListeners();
            upgradeMoveSpeedButton.onClick.AddListener(OnUpgradeMoveSpeedClicked);
        }

        RefreshGeneralStats();
        RefreshCargo();

        TransportMovement tm = currentWorker != null ? currentWorker.GetComponent<TransportMovement>() : null;
        if (tm != null && tm.RouteCells != null && tm.RouteCells.Count > 0)
        {
            if (RouteDrawer.Instance != null)
            {
                RouteDrawer.Instance.ShowRoute(tm.RouteCells);
            }
        }

        panel.SetActive(true);
    }

    public void Close()
    {
        if (RouteDrawer.Instance != null)
        {
            RouteDrawer.Instance.HideRoute();
        }

        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.SelectedSlotChanged -= OnInventorySlotSelected;
        }

        if (currentInventory != null)
        {
            currentInventory.OnOutputChanged -= RefreshCargo;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged -= RefreshStatus;
        }

        isSetupMode = false;
        currentWorker = null;
        currentInventory = null;
        currentTransportLogic = null;
        selectedItem = null;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void OnInventorySlotSelected()
    {
        if (!isSetupMode || currentWorker == null) return;

        InventoryObject obj = PlayerInventory.Instance.GetSelectedItem();

        if (obj != null && obj is ItemData itemData)
        {
            selectedItem = itemData;

            if (itemIcon != null)
            {
                itemIcon.enabled = true;
                itemIcon.sprite = selectedItem.icon;
            }

            if (itemNameText != null) itemNameText.text = selectedItem.objectName;
            if (statusText != null) statusText.text = "Haritada rotayı sürükleyerek çizin...";

            PlayerInventory.Instance.SelectedSlotChanged -= OnInventorySlotSelected;
            isSetupMode = false;

            if (workerManager != null)
            {
                workerManager.SetTransportMode(currentWorker, selectedItem);
            }
        }
    }

    private void RefreshGeneralStats()
    {
        if (currentWorker == null) return;

        if (nameText != null) nameText.text = currentWorker.Name;
        RefreshStatus();
        if (levelText != null) levelText.text = $"Level : {currentWorker.Level}";
        if (moveSpeedText != null) moveSpeedText.text = $"Move Speed : {currentWorker.MovementSpeed}";
    }

    private void RefreshStatus()
    {
        if (currentWorker == null) return;
        if (statusText != null && !isSetupMode) statusText.text = "Status : " + currentWorker.Status;
    }

    private void RefreshCargo()
    {
        if (currentInventory == null || currentWorker == null) return;

        if (cargoTotalText != null)
        {
            cargoTotalText.text = $"{currentInventory.GetOutputTotal()} / {currentInventory.MaxOutputCapacity}";
        }

        ItemData activeItem = (currentTransportLogic != null && currentTransportLogic.TransportItem != null)
            ? currentTransportLogic.TransportItem
            : selectedItem;

        if (itemIcon != null)
        {
            if (activeItem != null && activeItem.icon != null)
            {
                itemIcon.enabled = true;
                itemIcon.sprite = activeItem.icon;
            }
            else
            {
                itemIcon.enabled = false;
            }
        }

        if (itemNameText != null && activeItem != null)
        {
            itemNameText.text = activeItem.objectName;
        }

        RefreshStatus();
    }

    private void OnUpgradeMoveSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMovementSpeed(500))
        {
            RefreshGeneralStats();
        }
    }

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

    private void OnCancelSetupClicked()
    {
        Worker worker = currentWorker;
        Close();

        if (uiManager != null && worker != null)
        {
            uiManager.OpenWorkerUI(worker);
        }
    }
}
