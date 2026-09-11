using UnityEngine;
using System.Collections.Generic;

public class WorkerMovement : MonoBehaviour
{
    private Worker stats;

    public static HashSet<Vector3Int> OccupiedCells = new HashSet<Vector3Int>();

    [SerializeField] private Grid grid;
    [SerializeField] private Pathfinding pathfinding;

    public bool HasReachedTarget { get; private set; } = true;
    private bool hasClaimedCell = false;

    private List<Node> currentPath; 
    private int pathIndex; 
    private Vector3 currentWaypoint;
    private Vector3Int currentCell;


   

    private void Awake()
    {
        stats = GetComponent<Worker>();
    }

    public bool MoveTo(Vector3Int targetCell)
    {
        if (OccupiedCells.Contains(targetCell))
        {
            Debug.Log("bura dolu");
            return false;
        }

        Vector3Int startCell = grid.WorldToCell(transform.position);
        currentPath = pathfinding.FindPath(startCell, targetCell);

        
        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.Log("Oraya giden bir yol yok!");
            return false;
        }

        if (hasClaimedCell)
        {
            OccupiedCells.Remove(currentCell);
        }

        OccupiedCells.Add(targetCell);
        currentCell = targetCell;
        hasClaimedCell = true;

        // Start Move
        pathIndex = 0;
        currentWaypoint = grid.GetCellCenterWorld(currentPath[pathIndex].gridPosition);
        HasReachedTarget = false;
        stats.NotifyStatusChanged();
        return true;
    }

    public void StopMoving()
    {
        currentPath = null;
        HasReachedTarget = true;
        stats.NotifyStatusChanged();
    }

    private void Update()
    {
        if (HasReachedTarget)
            return;

        Move();
    }

    private void Move()
    {
        
        transform.position = Vector2.MoveTowards(
            transform.position,
            currentWaypoint,
            stats.MovementSpeed * Time.deltaTime
        );

        
        if (Vector2.Distance(transform.position, currentWaypoint) < 0.01f)
        {
            pathIndex++; 

            
            if (pathIndex >= currentPath.Count)
            {
                transform.position = currentWaypoint;
                HasReachedTarget = true;
                currentPath = null;
                stats.NotifyStatusChanged();
            }
            else
            {
                currentWaypoint = grid.GetCellCenterWorld(currentPath[pathIndex].gridPosition);
            }
        }
    }
}