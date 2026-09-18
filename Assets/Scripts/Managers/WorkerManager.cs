using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkerManager : MonoBehaviour
{
    private WorkerMovement selectedWorkerMovement;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask workableLayer;
    [SerializeField] private LayerMask workerLayer;
    [SerializeField] private WorkerUIManager uiManager;
    [SerializeField] private RouteDrawer routeDrawer;

    private bool MoveWorkerMode;
    private bool isDrawingRoute = false;
    private Worker pendingTransportWorker;
    private ItemData pendingTransportItem;

    public void SetMoveWorkerMode(bool tf)
    {
        MoveWorkerMode = tf;
    }

    public void HandleLeftClick(Vector2 mousePosition)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (isDrawingRoute)
        {
            return;
        }
        
        if (MoveWorkerMode)
        {
            TryMoveToWork(mousePosition);
            return;
        }

        TryOpenWorkerUI(mousePosition);

    }

    private void TryMoveToWork(Vector2 mousePosition) // controls at WorkerMovement.cs
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workableLayer);

        if (hit != null)
        {
            Vector3Int cell = grid.WorldToCell(mousePosition);
            
            if (selectedWorkerMovement.MoveTo(cell))
            {
                Worker worker = selectedWorkerMovement.GetComponent<Worker>();
                worker.CurrentState = WorkerState.Working;

                if (worker.Inventory != null && PlayerInventory.Instance != null)
                {
                    worker.Inventory.TransferAllToPlayer(PlayerInventory.Instance);
                }

                IInteractable interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    worker.CurrentWorkType = interactable.WorkType;
                }

                Debug.Log($"İş alanına gidiliyor! Görev: {worker.CurrentWorkType}");

                selectedWorkerMovement = null;
                MoveWorkerMode = false;
            }
            else
            {
                Debug.LogWarning("Uyarı: Seçilen hücre dolu veya ulaşılamıyor! Başka bir kare seçebilirsiniz.");
            }
        }
        else
        {
            Debug.Log("İptal: İşçi sadece belirlenen iş alanlarına gönderilebilir!");
            selectedWorkerMovement = null;
            MoveWorkerMode = false;
        }
    }

    private void TryOpenWorkerUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workerLayer);

        if (hit == null)
        {
            uiManager.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        Worker worker = hit.GetComponentInParent<Worker>();
        WorkerMovement movement = hit.GetComponentInParent<WorkerMovement>();

        if (worker == null)
        {
            uiManager.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        selectedWorkerMovement = movement;
        uiManager.OpenWorkerUI(worker);
    }

    public void SetTransportMode(Worker worker, ItemData item)
    {
        if (worker == null || item == null) return;

        pendingTransportWorker = worker;
        pendingTransportItem = item;
        isDrawingRoute = true;

        if (routeDrawer != null)
        {
            routeDrawer.StartDrawing(OnRouteCompleted, OnRouteCancelled);
        }
    }

    private void OnRouteCompleted(List<Vector3Int> route)
    {
        if (pendingTransportWorker != null && pendingTransportItem != null)
        {
            if (pendingTransportWorker.Inventory != null && PlayerInventory.Instance != null)
            {
                pendingTransportWorker.Inventory.TransferAllToPlayer(PlayerInventory.Instance);
            }

            pendingTransportWorker.StartTransporting(pendingTransportItem, route);
            Debug.Log($"{pendingTransportWorker.Name} taşıma görevine başladı! Taşınan: {pendingTransportItem.objectName}");
        }

        if (uiManager != null)
        {
            uiManager.CloseAllPanels();
        }

        pendingTransportWorker = null;
        pendingTransportItem = null;
        isDrawingRoute = false;
    }

    private void OnRouteCancelled()
    {
        Debug.Log("Rota çizimi iptal edildi.");

        if (uiManager != null)
        {
            uiManager.CloseAllPanels();
        }

        pendingTransportWorker = null;
        pendingTransportItem = null;
        isDrawingRoute = false;
    }
}