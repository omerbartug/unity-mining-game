using UnityEngine;

public enum WorkerState
{
    Idle,         
    Working,      
    Transporting  
}
public class Worker : MonoBehaviour
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

    private int storage;
    public int StoredItemCount => storage;

    private ItemData item;
    public ItemData CurrentItem => item;

}