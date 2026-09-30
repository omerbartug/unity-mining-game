using UnityEngine;
using System.Collections.Generic;

// İşçinin grid üzerinde A* algoritması ile hedefe yürümesini ve hücre rezervasyonunu yönetir.
public class WorkerMovement : MonoBehaviour
{
    private Worker stats;

    // Tüm işçilerin paylaştığı, rezerve edilmiş hedef hücreler (çakışmayı önler).
    public static HashSet<Vector3Int> OccupiedCells = new HashSet<Vector3Int>();

    private Grid grid;
    private Pathfinding pathfinding;

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

    // Eksik ızgara ve yol bulma referanslarını Singleton üzerinden çözer.
    private void Start()
    {
        if (pathfinding == null && Pathfinding.Instance != null)
        {
            pathfinding = Pathfinding.Instance;
        }

        if (grid == null && pathfinding != null)
        {
            grid = pathfinding.Grid;
        }
    }

    private void OnDestroy()
    {
        ReleaseClaim();
    }

    private void Update()
    {
        if (HasReachedTarget)
            return;

        Move();
    }

    // Hedef hücre boşsa ve yol bulunabiliyorsa hücreyi rezerve edip hareketi başlatır.
    public bool MoveTo(Vector3Int targetCell)
    {
        if (OccupiedCells.Contains(targetCell))
        {
            Debug.Log("Hedef hücre dolu!");
            return false;
        }

        Vector3Int startCell = grid.WorldToCell(transform.position);
        currentPath = pathfinding.FindPath(startCell, targetCell);

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.Log("Oraya giden bir yol yok!");
            return false;
        }

        // Önceki rezervasyonu bırak ve yeni hedefi rezerve et
        ReleaseClaim();
        OccupiedCells.Add(targetCell);
        currentCell = targetCell;
        hasClaimedCell = true;

        pathIndex = 0;
        currentWaypoint = grid.GetCellCenterWorld(currentPath[pathIndex].gridPosition);
        HasReachedTarget = false;
        stats.NotifyStatusChanged();
        return true;
    }

    // İşçinin hareketini durdurur, rotayı temizler ve hedef hücre rezervasyonunu serbest bırakır.
    public void StopMoving()
    {
        currentPath = null;
        HasReachedTarget = true;
        ReleaseClaim();
        stats.NotifyStatusChanged();
    }

    // İşçinin tuttuğu hedef hücre rezervasyonunu boşa çıkarır.
    public void ReleaseClaim()
    {
        if (hasClaimedCell)
        {
            OccupiedCells.Remove(currentCell);
            hasClaimedCell = false;
        }
    }

    // A* rotasındaki ara noktaları (waypoint) sırayla takip ederek hedefe yürütür.
    private void Move()
    {
        if (currentPath == null) return;

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