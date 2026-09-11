using UnityEngine;

public enum WorkerState
{
    Idle,         
    Working,      
    Transporting  
}
public class Worker : MonoBehaviour, IItemSource
{


    public WorkerState CurrentState { get; set; } = WorkerState.Idle;

    [SerializeField] private string name = "";
    public string Name => name;


    [SerializeField] private int level = 1;
    public int Level => level;


    [SerializeField] private float miningSpeed = 1f;
    public float MiningSpeed => miningSpeed;


    [SerializeField] private float movementSpeed = 2f;
    public float MovementSpeed => movementSpeed;
    
    [SerializeField] private int carryCapacity = 30;
    public int CarryCapacity => carryCapacity;

    private WorkerInventory workerInventory;
    public WorkerInventory Inventory => workerInventory;

    private WorkerInteraction workerInteraction;
    private WorkerMovement workerMovement;

    public event System.Action OnStatusChanged;

    public void NotifyStatusChanged()
    {
        OnStatusChanged?.Invoke();
    }

    public string Status
    {
        get
        {
            if (CurrentState == WorkerState.Idle) return "Idle";
            if (CurrentState == WorkerState.Transporting) return "Transporting";

            if (CurrentState == WorkerState.Working)
            {
                if (workerMovement != null && !workerMovement.HasReachedTarget)
                    return "Moving";

                if (workerInventory != null && workerInventory.IsFull())
                    return "Capacity Full";

                if (workerInteraction != null && !workerInteraction.IsInteracting)
                    return "No Input";

                return "Working";
            }

            return "";
        }
    }

    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
        workerInteraction = GetComponent<WorkerInteraction>();
        workerMovement = GetComponent<WorkerMovement>();
    }

    public void StopWorking()
    {
        CurrentState = WorkerState.Idle;

        if (workerMovement != null)
        {
            workerMovement.StopMoving();
        }

        NotifyStatusChanged();
    }

    public bool TryUpgradeMiningSpeed(int cost = 500, float amount = 0.5f)
    {
        if (PlayerStats.Instance.GetPlayerMoney() < cost)
        {
            Debug.Log("Yetersiz bakiye! Kazma hızı artırılamadı.");
            return false;
        }

        PlayerStats.Instance.RemoveMoney(cost);
        miningSpeed += amount;
        Debug.Log($"{name} kazma hızı arttı! Yeni hız: {miningSpeed}");
        return true;
    }

    public bool TryUpgradeMovementSpeed(int cost = 500, float amount = 0.5f)
    {
        if (PlayerStats.Instance.GetPlayerMoney() < cost)
        {
            Debug.Log("Yetersiz bakiye! Hareket hızı artırılamadı.");
            return false;
        }

        PlayerStats.Instance.RemoveMoney(cost);
        movementSpeed += amount;
        Debug.Log($"{name} hareket hızı arttı! Yeni hız: {movementSpeed}");
        return true;
    }

    public void CollectItems(Inventory targetInventory)
    {
        if (workerInventory != null && targetInventory is PlayerInventory playerInventory)
        {
            if (workerInventory.OutputItems.Count == 0) return;

            foreach (var pair in workerInventory.OutputItems)
            {
                playerInventory.AddItem(pair.Key, pair.Value);
            }

            workerInventory.ClearOutput();
        }
    }
}