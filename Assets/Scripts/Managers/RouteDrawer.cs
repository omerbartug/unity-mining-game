using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RouteDrawer : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private NodeMaker nodeMaker;
    [SerializeField] private LineRenderer lineRenderer;

    private List<Vector3Int> routeCells = new List<Vector3Int>();
    private bool isDrawing = false;
    private bool isActive = false;

    private Action<List<Vector3Int>> onRouteCompleted;
    private Action onRouteCancelled;

    public static RouteDrawer Instance { get; private set; }

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

        if (grid == null) grid = FindFirstObjectByType<Grid>();
        if (nodeMaker == null) nodeMaker = FindFirstObjectByType<NodeMaker>();
        if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
    }

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

    public void HideRoute()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    public void StartDrawing(Action<List<Vector3Int>> onCompleted, Action onCancelled)
    {
        isActive = true;
        isDrawing = false;
        onRouteCompleted = onCompleted;
        onRouteCancelled = onCancelled;

        routeCells.Clear();
        HideRoute();
    }

    public void CancelDrawing()
    {
        isActive = false;
        isDrawing = false;
        routeCells.Clear();

        HideRoute();

        onRouteCancelled?.Invoke();
    }

    private void Update()
    {
        if (!isActive) return;

        // Sağ tık: Çizimi iptal et
        if (Input.GetMouseButtonDown(1))
        {
            CancelDrawing();
            return;
        }

        // Sol tık basıldı: Çizimi başlat
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = grid.WorldToCell(mouseWorld);

            if (IsWalkable(cell))
            {
                isDrawing = true;
                routeCells.Clear();
                if (lineRenderer != null) lineRenderer.positionCount = 0;

                routeCells.Add(cell);
                AppendCellToLine(cell);
            }
        }

        // Sol tık basılı tutuluyor: Sürükleyerek çizmeye devam et
        if (Input.GetMouseButton(0) && isDrawing)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cell = grid.WorldToCell(mouseWorld);

            if (routeCells.Count > 0 && cell != routeCells[routeCells.Count - 1])
            {
                TryAddCellsTo(cell);
            }
        }

        // Sol tık bırakıldı: Çizimi tamamla
        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
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
    }

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

    private bool IsWalkable(Vector3Int cell)
    {
        if (nodeMaker == null) return true;
        Node node = nodeMaker.GetNode(cell);
        return node != null && node.isWalkable;
    }

    private void AppendCellToLine(Vector3Int cell)
    {
        if (lineRenderer == null || grid == null) return;

        lineRenderer.positionCount++;
        Vector3 worldPos = grid.GetCellCenterWorld(cell);
        worldPos.z = -1f; // Tilemap'in önünde görünmesi için
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, worldPos);
    }
}
