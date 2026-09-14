using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OperatingWorkerPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Genel Bilgiler")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text workSpeedText;
    [SerializeField] private TMP_Text moveSpeedText;

    [Header("Input Storage")]
    [SerializeField] private Image inputIcon;
    [SerializeField] private TMP_Text inputTotalText;

    [Header("Butonlar")]
    [SerializeField] private Button stopButton;
    [SerializeField] private Button upgradeWorkSpeedButton;
    [SerializeField] private Button upgradeMoveSpeedButton;

    [Header("Girdi Butonları (Opsiyonel)")]
    [SerializeField] private Button addInputButton;
    [SerializeField] private Button retrieveInputButton;

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
            currentInventory.OnInputChanged += RefreshInput;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged += RefreshStatus;
        }

        PlayerInteraction.OnNearbyWorkerChanged += HandleNearbyWorkerChanged;

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

        RefreshGeneralStats();
        RefreshInput();
        UpdateButtonStates();

        panel.SetActive(true);
    }

    public void Close()
    {
        if (currentInventory != null)
        {
            currentInventory.OnInputChanged -= RefreshInput;
        }

        if (currentWorker != null)
        {
            currentWorker.OnStatusChanged -= RefreshStatus;
        }

        PlayerInteraction.OnNearbyWorkerChanged -= HandleNearbyWorkerChanged;

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

    private void RefreshInput()
    {
        if (currentInventory == null || currentWorker == null) return;

        if (inputTotalText != null)
        {
            inputTotalText.text = $"{currentInventory.GetInputTotal()} / {currentInventory.MaxInputCapacity}";
        }

        if (inputIcon != null)
        {
            InventoryObject firstItem = null;
            foreach (var key in currentInventory.InputItems.Keys)
            {
                firstItem = key;
                break;
            }

            if (firstItem != null && firstItem.icon != null)
            {
                inputIcon.enabled = true;
                inputIcon.sprite = firstItem.icon;
            }
            else
            {
                inputIcon.enabled = false;
            }
        }

        RefreshStatus();
        UpdateButtonStates();
    }

    private bool IsPlayerNearby()
    {
        return PlayerInteraction.Instance != null && PlayerInteraction.Instance.CurrentNearbyWorker == currentWorker;
    }

    private void HandleNearbyWorkerChanged()
    {
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        bool isNearby = IsPlayerNearby();
        if (addInputButton != null) addInputButton.interactable = isNearby;
        if (retrieveInputButton != null) retrieveInputButton.interactable = isNearby && (currentInventory != null && currentInventory.GetInputTotal() > 0);
    }

    private void OnAddInputClicked()
    {
        if (!IsPlayerNearby() || currentInventory == null) return;
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

    private void OnRetrieveInputClicked()
    {
        if (!IsPlayerNearby() || currentInventory == null) return;
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
