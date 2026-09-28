using UnityEngine;
using System.Collections.Generic;

// İşçinin ana yaşam döngüsü durumlarını belirtir.
public enum WorkerState
{
    Idle,         
    Working,      
    Transporting  
}

// İşçinin bir iş alanında (IInteractable) icra ettiği görev türünü belirtir.
public enum WorkerWorkType
{
    None,
    Mining,
    Processing,
    Operating
}

public class Worker : MonoBehaviour, IItemSource
{
    public WorkerState CurrentState { get; set; } = WorkerState.Idle;

    private WorkerWorkType currentWorkType = WorkerWorkType.None;
    public WorkerWorkType CurrentWorkType
    {
        get => currentWorkType;
        set
        {
            currentWorkType = value;
            UpdateInventoryCapacities();
        }
    }

    [SerializeField] private new string name = "";
    public string Name => name;

    [SerializeField] private int level = 1;
    public int Level => level;

    [SerializeField] private float miningSpeed = 1f;
    public float MiningSpeed => miningSpeed;

    [SerializeField] private float movementSpeed = 2f;
    public float MovementSpeed => movementSpeed;

    [SerializeField] private int carryCapacity = 30;
    public int CarryCapacity => carryCapacity;

    [Header("Upgrade Settings")]
    [SerializeField] private int miningSpeedUpgradeCost = 500;
    [SerializeField] private float miningSpeedUpgradeAmount = 0.5f;
    [SerializeField] private int movementSpeedUpgradeCost = 500;
    [SerializeField] private float movementSpeedUpgradeAmount = 0.5f;

    public int MiningSpeedUpgradeCost => miningSpeedUpgradeCost;
    public float MiningSpeedUpgradeAmount => miningSpeedUpgradeAmount;
    public int MovementSpeedUpgradeCost => movementSpeedUpgradeCost;
    public float MovementSpeedUpgradeAmount => movementSpeedUpgradeAmount;

    private WorkerInventory workerInventory;
    public WorkerInventory Inventory => workerInventory;

    private WorkerInteraction workerInteraction;
    private WorkerMovement workerMovement;
    private TransportMovement transportMovement;
    private TransportLogic transportLogic;

    public event System.Action OnStatusChanged;

    // UI panellerine işçinin durumunun güncellendiğini bildirir.
    public void NotifyStatusChanged() => OnStatusChanged?.Invoke();

    // UI panellerinde gösterilecek anlık durum metnini hiyerarşik olarak belirler.
    public string Status
    {
        get
        {
            if (CurrentState == WorkerState.Idle) return "Idle";
            
            if (CurrentState == WorkerState.Transporting)
            {
                if (transportMovement != null && transportMovement.IsMovingToStart)
                    return "Moving to Route";
                return "Transporting";
            }

            if (CurrentState == WorkerState.Working)
            {
                if (workerMovement != null && !workerMovement.HasReachedTarget)
                    return "Moving";

                if (workerInventory != null && workerInventory.IsFull())
                    return "Capacity Full";

                if (workerInteraction != null && !workerInteraction.IsInteracting)
                {
                    if (CurrentWorkType == WorkerWorkType.Processing || CurrentWorkType == WorkerWorkType.Operating)
                        return "No Input";

                    return "Idle";
                }

                return CurrentWorkType switch
                {
                    WorkerWorkType.Mining => "Mining",
                    WorkerWorkType.Processing => "Processing",
                    WorkerWorkType.Operating => "Operating",
                    _ => "Working"
                };
            }

            return "";
        }
    }

    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
        workerInteraction = GetComponent<WorkerInteraction>();
        workerMovement = GetComponent<WorkerMovement>();
        transportMovement = GetComponent<TransportMovement>();
        transportLogic = GetComponent<TransportLogic>();

        UpdateInventoryCapacities();
    }


    // İşçinin mevcut rolüne göre envanter hazne kapasitelerini belirler.
    public void UpdateInventoryCapacities()
    {
        if (workerInventory == null) return;

        if (CurrentState == WorkerState.Transporting)
        {
            workerInventory.SetCapacities(inputCap: 0, outputCap: carryCapacity);
            return;
        }

        switch (currentWorkType)
        {
            case WorkerWorkType.Mining:
                workerInventory.SetCapacities(inputCap: 0, outputCap: carryCapacity);
                break;
            case WorkerWorkType.Processing:
                workerInventory.SetCapacities(inputCap: carryCapacity / 2, outputCap: carryCapacity / 2);
                break;
            case WorkerWorkType.Operating:
                workerInventory.SetCapacities(inputCap: carryCapacity, outputCap: 0);
                break;
            default:
                workerInventory.SetCapacities(inputCap: carryCapacity / 2, outputCap: carryCapacity / 2);
                break;
        }
    }

    // İşçiyi taşıyıcı rolüne geçirir, rotasını ve taşınacak eşyayı yapılandırır.
    public void StartTransporting(ItemData item, List<Vector3Int> route)
    {
        CurrentState = WorkerState.Transporting;
        CurrentWorkType = WorkerWorkType.None;

        if (workerMovement != null) workerMovement.StopMoving();
        if (transportLogic != null) transportLogic.SetTransportItem(item);
        if (workerInventory != null) workerInventory.SetTransportFilter(item);
        if (transportMovement != null) transportMovement.SetRoute(route);

        NotifyStatusChanged();
    }

    // İşçiyi boşa çıkarır (Idle), tüm hareket ve taşıma işlemlerini durdurup filtreleri temizler.
    public void StopWorking()
    {
        CurrentState = WorkerState.Idle;
        CurrentWorkType = WorkerWorkType.None;

        if (workerMovement != null) workerMovement.StopMoving();
        if (transportMovement != null) transportMovement.StopPatrol();
        if (transportLogic != null) transportLogic.ClearTransportItem();
        if (workerInventory != null) workerInventory.ClearTransportFilter();

        NotifyStatusChanged();
    }

    // Oyuncunun parası yeterliyse işçinin maden kazma hızını yükseltir.
    public bool TryUpgradeMiningSpeed()
    {
        return TryUpgradeMiningSpeed(miningSpeedUpgradeCost, miningSpeedUpgradeAmount);
    }

    // Dışarıdan özel parametrelerle kazma hızı artırımı yapmayı sağlar.
    public bool TryUpgradeMiningSpeed(int cost, float amount = 0.5f)
    {
        if (!PlayerStats.Instance.TrySpendMoney(cost))
        {
            Debug.Log("Yetersiz bakiye! Kazma hızı artırılamadı.");
            return false;
        }

        miningSpeed += amount;
        Debug.Log($"{name} kazma hızı arttı! Yeni hız: {miningSpeed}");
        return true;
    }

    // Oyuncunun parası yeterliyse işçinin hareket hızını yükseltir.
    public bool TryUpgradeMovementSpeed()
    {
        return TryUpgradeMovementSpeed(movementSpeedUpgradeCost, movementSpeedUpgradeAmount);
    }

    // Dışarıdan özel parametrelerle hareket hızı artırımı yapmayı sağlar.
    public bool TryUpgradeMovementSpeed(int cost, float amount = 0.5f)
    {
        if (!PlayerStats.Instance.TrySpendMoney(cost))
        {
            Debug.Log("Yetersiz bakiye! Hareket hızı artırılamadı.");
            return false;
        }

        movementSpeed += amount;
        Debug.Log($"{name} hareket hızı arttı! Yeni hız: {movementSpeed}");
        return true;
    }

    // İşçinin çıkış haznesindeki ürünleri gelen envantere (Oyuncu veya Taşıyıcı İşçi) kayıpsız aktarır.
    public void CollectItems(Inventory targetInventory)
    {
        if (workerInventory == null || targetInventory == null) return;

        // Taşıyıcı modundaki bir işçiden başka bir taşıyıcı eşya toplayamaz
        if (this.CurrentState == WorkerState.Transporting) return;

        var itemsToCollect = new List<KeyValuePair<InventoryObject, int>>(workerInventory.OutputItems);
        foreach (var pair in itemsToCollect)
        {
            if (targetInventory.CanAccept(pair.Key))
            {
                int added = targetInventory.AddItem(pair.Key, pair.Value);
                if (added > 0)
                {
                    workerInventory.RemoveFromOutput(pair.Key, added);
                }
            }
        }
    }
}