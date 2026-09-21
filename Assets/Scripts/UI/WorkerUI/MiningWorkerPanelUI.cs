using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MiningWorkerPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Genel Bilgiler")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text workSpeedText;
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Output Storage")]
    [SerializeField] private Image outputIcon;
    [SerializeField] private TMP_Text outputTotalText;

    [Header("Butonlar")]
    [SerializeField] private Button stopButton;
    [SerializeField] private Button upgradeWorkSpeedButton;
    [SerializeField] private Button upgradeMoveSpeedButton;

    [Header("Referanslar")]
    [SerializeField] private WorkerUIManager uiManager;

    private Worker currentWorker;
    private WorkerInventory currentInventory;

    public void Open(Worker worker)
   {
        currentWorker = worker;
        currentInventory = worker != null ? worker.Inventory : null;

        if (currentInventory != null)
        {
            currentInventory.OnOutputChanged += RefreshOutput;
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

        if (upgradeWorkSpeedButton != null)
        {
            upgradeWorkSpeedButton.onClick.RemoveAllListeners();
            upgradeWorkSpeedButton.onClick.AddListener(OnUpgradeWorkSpeedClicked);
        }

        if (upgradeMoveSpeedButton != null)
        {
            upgradeMoveSpeedButton.onClick.RemoveAllListeners();
            upgradeMoveSpeedButton.onClick.AddListener(OnUpgradeMoveSpeedClicked);
        }

        RefreshGeneralStats();
        RefreshOutput();

        panel.SetActive(true);
    }

    public void Close()
    {
        if (currentInventory != null)
        {
            currentInventory.OnOutputChanged -= RefreshOutput;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged -= RefreshStatus;
        }

        currentWorker = null;
        currentInventory = null;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void RefreshGeneralStats()
    {
        if (currentWorker == null) return;

        if (nameText != null) nameText.text = currentWorker.Name;
        RefreshStatus();
        if (levelText != null) levelText.text = $"Level : {currentWorker.Level}";
        if (workSpeedText != null) workSpeedText.text = $"Work Speed : {currentWorker.MiningSpeed}";
        if (moveSpeedText != null) moveSpeedText.text = $"Move Speed : {currentWorker.MovementSpeed}";
    }

    private void RefreshStatus()
    {
        if (currentWorker == null) return;
        if (statusText != null) statusText.text = "Status : " + currentWorker.Status;
    }

    private void RefreshOutput()
    {
        if (currentInventory == null || currentWorker == null) return;

        if (outputTotalText != null)
        {
            outputTotalText.text = $"{currentInventory.GetOutputTotal()} / {currentInventory.OutputCapacity}";
        }

        if (outputIcon != null)
        {
            InventoryObject firstItem = null;
            foreach (var key in currentInventory.OutputItems.Keys)
            {
                firstItem = key;
                break;
            }

            if (firstItem != null && firstItem.icon != null)
            {
                outputIcon.enabled = true;
                outputIcon.sprite = firstItem.icon;
            }
            else
            {
                outputIcon.enabled = false;
            }
        }

        RefreshStatus();
    }

    private void OnUpgradeWorkSpeedClicked()
    {
        if (currentWorker == null) return;
        if (currentWorker.TryUpgradeMiningSpeed(500))
        {
            RefreshGeneralStats();
        }
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
}
