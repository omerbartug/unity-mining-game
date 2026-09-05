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
    
    [SerializeField] private int carryCapacity = 10;
    public int CarryCapacity => carryCapacity;

    private WorkerInventory workerInventory;
    public WorkerInventory Inventory => workerInventory;

    public string Status
    {
        get
        {
            if (CurrentState == WorkerState.Idle) return "Idle";
            if (CurrentState == WorkerState.Working && workerInventory != null && workerInventory.IsFull()) return "Capacity Full";
            if (CurrentState == WorkerState.Working) return "Working";
            if (CurrentState == WorkerState.Transporting) return "Transporting";
            return "";
        }
    }

    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
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
        if (workerInventory != null)
        {
            workerInventory.TransferAllItemsTo(targetInventory);
        }
    }
}