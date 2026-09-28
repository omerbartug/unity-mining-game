using System.Collections.Generic;
using UnityEngine;

// Taşıyıcı işçinin rota üzerindeki devriye hareketini ve başlangıç noktasına intikalini yönetir.
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

    // Gerekli bileşen referansını önbelleğe alır ve eksik atamaları denetler.
    private void Awake()
    {
        worker = GetComponent<Worker>();
    }

    // Belirtilen rotayı kaydeder; başlangıç noktasındaysa devriyeyi başlatır, değilse oraya A* ile yol bulur.
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

        Vector3Int currentCell = grid.WorldToCell(transform.position);

        // Zaten rotanın ilk noktasındaysa direkt devriyeye başla
        if (currentCell == routeCells[0])
        {
            StartPatrol();
            return;
        }

        // Başka bir yerdeyse, başlangıç noktasına A* ile yol bul
        pathToStart = pathfinding.FindPath(currentCell, routeCells[0]);

        if (pathToStart != null && pathToStart.Count > 0)
        {
            isMovingToStart = true;
            isPatrolling = false;
            pathToStartIndex = 0;
            currentTarget = grid.GetCellCenterWorld(pathToStart[pathToStartIndex].gridPosition);
        }
        else
        {
            Debug.LogWarning($"{worker?.Name}: Rota başlangıç noktasına ulaşılamıyor!");
            StopPatrol();
        }
    }

    // Taşıma devriyesini durdurur, rotayı ve geçici yolları sıfırlar.
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

    // Rota başlangıcına ulaşıldığında devriye durumunu ve ilk hedef noktayı kurar.
    private void StartPatrol()
    {
        isMovingToStart = false;
        isPatrolling = true;
        routeIndex = (routeCells.Count > 1) ? 1 : 0;
        currentTarget = grid.GetCellCenterWorld(routeCells[routeIndex]);
        if (worker != null) worker.NotifyStatusChanged();
    }

    // Taşıyıcı durumunu kontrol eder; başlangıca intikali veya devriye hareketini işletir.
    private void Update()
    {
        if (worker != null && worker.CurrentState != WorkerState.Transporting)
        {
            if (isPatrolling || isMovingToStart) StopPatrol();
            return;
        }

        if (isMovingToStart)
        {
            MoveAlongPathToStart();
            return;
        }

        if (isPatrolling && routeCells != null && routeCells.Count > 1)
        {
            MoveAlongPatrolRoute();
        }
    }

    // Rota başlangıç noktasına doğru A* yolu üzerinden adım adım yürütür.
    private void MoveAlongPathToStart()
    {
        if (MoveTowards(currentTarget))
        {
            pathToStartIndex++;

            if (pathToStartIndex >= pathToStart.Count)
            {
                StartPatrol();
            }
            else
            {
                currentTarget = grid.GetCellCenterWorld(pathToStart[pathToStartIndex].gridPosition);
            }
        }
    }

    // Devriye rotasındaki hedef hücreye doğru hareket eder.
    private void MoveAlongPatrolRoute()
    {
        if (MoveTowards(currentTarget))
        {
            AdvanceToNextWaypoint();
        }
    }

    // Rota üzerinde iki uç arasında ileri-geri (ping-pong) devriye indeksini ilerletir.
    private void AdvanceToNextWaypoint()
    {
        if (routeCells.Count <= 1) return;

        routeIndex += direction;

        // Sona geldiysek yönü geri çevir (-1)
        if (routeIndex >= routeCells.Count)
        {
            direction = -1;
            routeIndex = routeCells.Count - 2;
        }
        // Başa geldiysek yönü ileri çevir (+1)
        else if (routeIndex < 0)
        {
            direction = 1;
            routeIndex = 1;
        }

        routeIndex = Mathf.Clamp(routeIndex, 0, routeCells.Count - 1);
        currentTarget = grid.GetCellCenterWorld(routeCells[routeIndex]);
    }

    // Verilen hedefe doğru adım atar, hedefe ulaşıldığında true döner.
    private bool MoveTowards(Vector3 target)
    {
        float speed = worker != null ? worker.MovementSpeed : 2f;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.01f)
        {
            transform.position = target;
            return true;
        }

        return false;
    }
}
