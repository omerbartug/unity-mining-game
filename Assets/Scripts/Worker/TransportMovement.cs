using System.Collections.Generic;
using UnityEngine;

public class TransportMovement : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Pathfinding pathfinding;

    private Worker worker;
    private List<Vector3Int> routeCells = new List<Vector3Int>();
    private int routeIndex = 0;
    private int direction = 1; // +1: ileri, -1: geri
    private Vector3 currentTarget;
    private bool isPatrolling = false;

    private bool isMovingToStart = false;
    private List<Node> pathToStart;
    private int pathToStartIndex = 0;

    public bool IsPatrolling => isPatrolling;
    public bool IsMovingToStart => isMovingToStart;
    public List<Vector3Int> RouteCells => routeCells;

    private void Awake()
    {
        worker = GetComponent<Worker>();

        // Eğer Inspector'dan Grid atanmadıysa sahneden otomatik bulalım
        if (grid == null)
        {
            grid = FindFirstObjectByType<Grid>();
        }

        if (pathfinding == null)
        {
            pathfinding = FindFirstObjectByType<Pathfinding>();
        }
    }

    public void SetRoute(List<Vector3Int> cells)
    {
        if (cells == null || cells.Count == 0)
        {
            StopPatrol();
            return;
        }

        routeCells = new List<Vector3Int>(cells);
        routeIndex = 0;
        direction = 1;

        if (grid == null)
        {
            StopPatrol();
            return;
        }

        Vector3Int currentCell = grid.WorldToCell(transform.position);

        // Eğer işçi zaten rotanın ilk noktasındaysa direkt devriyeye başla
        if (currentCell == routeCells[0])
        {
            isMovingToStart = false;
            isPatrolling = true;
            routeIndex = (routeCells.Count > 1) ? 1 : 0;
            currentTarget = grid.GetCellCenterWorld(routeCells[routeIndex]);
            return;
        }

        // İşçi başka bir yerdeyse, rotanın ilk noktasına pathfinding ile yol bul
        if (pathfinding != null)
        {
            pathToStart = pathfinding.FindPath(currentCell, routeCells[0]);
        }

        if (pathToStart != null && pathToStart.Count > 0)
        {
            isMovingToStart = true;
            isPatrolling = false;
            pathToStartIndex = 0;
            currentTarget = grid.GetCellCenterWorld(pathToStart[pathToStartIndex].gridPosition);
        }
        else
        {
            Debug.LogWarning($"{worker.Name}: Rota başlangıç noktasına ulaşılamıyor!");
            StopPatrol();
        }
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        isMovingToStart = false;
        pathToStart = null;
        pathToStartIndex = 0;
        routeCells.Clear();
        routeIndex = 0;
        direction = 1;
    }

    private void Update()
    {
        if (worker != null && worker.CurrentState != WorkerState.Transporting)
        {
            if (isPatrolling || isMovingToStart)
            {
                StopPatrol();
            }
            return;
        }

        if (isMovingToStart)
        {
            MoveTowardsStart();
            return;
        }

        if (!isPatrolling || routeCells == null || routeCells.Count <= 1)
            return;

        MoveTowardsTarget();
    }

    private void MoveTowardsStart()
    {
        float speed = worker != null ? worker.MovementSpeed : 2f;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentTarget) < 0.01f)
        {
            transform.position = currentTarget;
            pathToStartIndex++;

            if (pathToStartIndex >= pathToStart.Count)
            {
                isMovingToStart = false;
                isPatrolling = true;
                routeIndex = (routeCells.Count > 1) ? 1 : 0;
                currentTarget = grid.GetCellCenterWorld(routeCells[routeIndex]);
                if (worker != null) worker.NotifyStatusChanged();
            }
            else
            {
                currentTarget = grid.GetCellCenterWorld(pathToStart[pathToStartIndex].gridPosition);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        float speed = worker != null ? worker.MovementSpeed : 2f;

        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget,
            speed * Time.deltaTime
        );

        // Hedefe ulaşıldı mı kontrolü
        if (Vector2.Distance(transform.position, currentTarget) < 0.01f)
        {
            transform.position = currentTarget;
            AdvanceToNextWaypoint();
        }
    }

    private void AdvanceToNextWaypoint()
    {
        // Rota tek noktadan ibaretse hareket gerekmez
        if (routeCells.Count <= 1) return;

        routeIndex += direction;

        // Sona geldiysek yönü tersine çevir (-1)
        if (routeIndex >= routeCells.Count)
        {
            direction = -1;
            routeIndex = routeCells.Count - 2; // Bir önceki noktaya dön
        }
        // Başa geldiysek yönü ileri çevir (+1)
        else if (routeIndex < 0)
        {
            direction = 1;
            routeIndex = 1; // İkinci noktaya doğru git
        }

        // Sınır güvenliği (tekrar taşma olmaması için)
        routeIndex = Mathf.Clamp(routeIndex, 0, routeCells.Count - 1);
        currentTarget = grid.GetCellCenterWorld(routeCells[routeIndex]);
    }
}
