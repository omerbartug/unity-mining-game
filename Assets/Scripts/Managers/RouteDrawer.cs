using System;
using System.Collections.Generic;
using UnityEngine;

// Taşıyıcı işçiler için harita üzerinde çizgi ile yürünebilir rota çizen ve yöneten bileşendir.
public class RouteDrawer : MonoBehaviour
{
    public static RouteDrawer Instance { get; private set; }

    [SerializeField] private Grid grid;
    [SerializeField] private NodeMaker nodeMaker;
    [SerializeField] private LineRenderer lineRenderer;

    private readonly List<Vector3Int> routeCells = new List<Vector3Int>();
    private bool isDrawing = false;
    private bool isActive = false;

    private Action<List<Vector3Int>> onRouteCompleted;
    private Action onRouteCancelled;

    public bool IsActive => isActive;
    public bool IsDrawing => isDrawing;

    // Singleton örneğini ve ızgara bileşenlerini hazırlar.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (grid == null && Pathfinding.Instance != null) grid = Pathfinding.Instance.Grid;
        if (grid == null) grid = FindFirstObjectByType<Grid>();

        if (nodeMaker == null && Pathfinding.Instance != null) nodeMaker = Pathfinding.Instance.NodeMaker;
        if (nodeMaker == null) nodeMaker = FindFirstObjectByType<NodeMaker>();
        
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
    }

    // Belirtilen hücre listesini LineRenderer ile sahnede görselleştirir.
    public void ShowRoute(List<Vector3Int> cells)
    {
        if (lineRenderer == null || grid == null || cells == null || cells.Count == 0)
        {
            HideRoute();
            return;
        }

        lineRenderer.positionCount = cells.Count;
        for (int i = 0; i < cells.Count; i++)
        {
            Vector3 worldPos = grid.GetCellCenterWorld(cells[i]);
            worldPos.z = -1f; // Tilemap'in önünde görünmesi için
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    // Çizilmiş olan rotayı gizler ve çizgi noktalarını temizler.
    public void HideRoute()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    // Rota çizim modunu aktif hale getirir ve tamamlanma/iptal callback'lerini kaydeder.
    public void StartDrawing(Action<List<Vector3Int>> onCompleted, Action onCancelled)
    {
        isActive = true;
        isDrawing = false;
        onRouteCompleted = onCompleted;
        onRouteCancelled = onCancelled;

        routeCells.Clear();
        HideRoute();
    }

    // Aktif çizimi iptal eder, rotayı temizler ve iptal callback'ini tetikler.
    public void CancelDrawing()
    {
        isActive = false;
        isDrawing = false;
        routeCells.Clear();

        HideRoute();

        onRouteCancelled?.Invoke();
    }

    // Sol tık basıldığında tıklanan hücre yürünebilirse çizimi başlatır.
    public void HandlePointerDown(Vector2 worldPos)
    {
        if (!isActive || grid == null) return;

        Vector3Int cell = grid.WorldToCell(worldPos);
        if (IsWalkable(cell))
        {
            isDrawing = true;
            routeCells.Clear();
            if (lineRenderer != null) lineRenderer.positionCount = 0;

            routeCells.Add(cell);
            AppendCellToLine(cell);
        }
    }

    // Fare basılı tutularak sürüklendiğinde rotayı ara hücrelerle genişletir.
    public void HandlePointerDrag(Vector2 worldPos)
    {
        if (!isActive || !isDrawing || grid == null) return;

        Vector3Int cell = grid.WorldToCell(worldPos);
        if (routeCells.Count > 0 && cell != routeCells[routeCells.Count - 1])
        {
            TryAddCellsTo(cell);
        }
    }

    // Sol tık bırakıldığında rota geçerliyse tamamlar, değilse iptal eder.
    public void HandlePointerUp()
    {
        if (!isActive || !isDrawing) return;

        isDrawing = false;
        isActive = false;

        if (routeCells.Count >= 2)
        {
            List<Vector3Int> completedRoute = new List<Vector3Int>(routeCells);
            HideRoute();
            onRouteCompleted?.Invoke(completedRoute);
        }
        else
        {
            CancelDrawing();
        }
    }

    // Mevcut hücreden hedef hücreye kadar yürünebilir hücreleri rotaya ve çizgiye ekler.
    private void TryAddCellsTo(Vector3Int target)
    {
        Vector3Int current = routeCells[routeCells.Count - 1];

        while (current != target)
        {
            int stepX = 0;
            int stepY = 0;

            if (current.x != target.x)
                stepX = (target.x > current.x) ? 1 : -1;
            else if (current.y != target.y)
                stepY = (target.y > current.y) ? 1 : -1;

            Vector3Int next = new Vector3Int(current.x + stepX, current.y + stepY, 0);

            // Eğer yol üstünde yürünemeyen bir engel varsa orada dur
            if (!IsWalkable(next))
                break;

            // Zaten rotada yoksa listeye ve çizgiye ekle
            if (!routeCells.Contains(next))
            {
                routeCells.Add(next);
                AppendCellToLine(next);
            }

            current = next;
        }
    }

    // Belirtilen hücrenin yürünebilir olup olmadığını doğrular.
    private bool IsWalkable(Vector3Int cell)
    {
        if (nodeMaker == null) return true;
        Node node = nodeMaker.GetNode(cell);
        return node != null && node.isWalkable;
    }

    // Verilen hücreyi LineRenderer pozisyon listesine ekler.
    private void AppendCellToLine(Vector3Int cell)
    {
        if (lineRenderer == null || grid == null) return;

        lineRenderer.positionCount++;
        Vector3 worldPos = grid.GetCellCenterWorld(cell);
        worldPos.z = -1f; // Tilemap'in önünde görünmesi için
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);
    }
}
