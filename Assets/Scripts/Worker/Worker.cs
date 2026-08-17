using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private int level = 1;

    [SerializeField] private float miningSpeed = 1f;

    [SerializeField] private float movementSpeed = 2f;
    public float MovementSpeed => movementSpeed;
    
    [SerializeField] private int carryCapacity = 10;
    public int CarryCapacity => carryCapacity;
}