using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// İşçi seçimi, yerleşimi, görev ataması ve taşıma rotalarını yöneten koordinasyon bileşenidir.
public class WorkerManager : MonoBehaviour, IObjectInputManager
{
    [Header("Dependencies")]
    [SerializeField] private GhostPreview ghostPreview;
    private PlayerInventory inventory;
    private Grid grid;
    [SerializeField] private LayerMask workableLayer;
    [SerializeField] private LayerMask workerLayer;
    [SerializeField] private WorkerUIManager uiManager;
    private RouteDrawer routeDrawer;

    private WorkerData selectedWorkerData;
    private WorkerMovement selectedWorkerMovement;

    private bool isMoveWorkerMode;
    private bool isDrawingRoute = false;
    private Worker pendingTransportWorker;
    private ItemData pendingTransportItem;

    // Gerekli bileşenleri hazırlar ve hayalet önizleme bileşenini doğrular.
    private void Awake()
    {
        if (ghostPreview == null)
            ghostPreview = GetComponent<GhostPreview>() ?? gameObject.AddComponent<GhostPreview>();

        if (grid == null && Pathfinding.Instance != null)
            grid = Pathfinding.Instance.Grid;

        if (routeDrawer == null)
            routeDrawer = RouteDrawer.Instance;
    }

    // Event aboneliklerini kurar.
    private void Start()
    {
        if (ghostPreview != null)
            ghostPreview.OnPlacementConfirmed += HandlePlaced;

        if (inventory == null)
            inventory = PlayerInventory.Instance;

        if (inventory != null)
        {
            inventory.SelectedSlotChanged += UpdatePlacementMode;
            inventory.InventoryChanged += UpdatePlacementMode;
        }
    }

    // Bellek sızıntılarını önlemek için event aboneliklerini temizler.
    private void OnDestroy()
    {
        if (ghostPreview != null)
            ghostPreview.OnPlacementConfirmed -= HandlePlaced;

        if (inventory != null)
        {
            inventory.SelectedSlotChanged -= UpdatePlacementMode;
            inventory.InventoryChanged -= UpdatePlacementMode;
        }
    }

    // Yerleştirme modundaysa farenin anlık pozisyonunu hayalet önizlemeye iletir.
    private void Update()
    {
        if (!IsPlacementMode) return;

        if (PlayerInputManager.Instance != null && ghostPreview != null)
        {
            ghostPreview.UpdatePreview(PlayerInputManager.Instance.MouseWorldPosition);
        }
    }

    // Hayalet önizleme üzerinden işçi yerleştirme modunu başlatır.
    public void SelectWorker(WorkerData worker)
    {
        selectedWorkerData = worker;
        if (selectedWorkerData == null || ghostPreview == null) return;

        ghostPreview.Show(
            selectedWorkerData.ghostPrefab,
            selectedWorkerData.size,
            selectedWorkerData.placementBlockerLayer,
            selectedWorkerData.fineLayer
        );
    }

    /// INTERFACE METODLARI
    /// -----------------------------------------------------------------------------------

    // Fare sol tıkını yerleştirme, işe yönlendirme veya işçi paneli açma amacıyla işler.
    public void HandleLeftClick(Vector2 mousePosition)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (isDrawingRoute)
            return;

        if (IsPlacementMode)
        {
            ghostPreview.TryConfirmPlacement();
            return;
        }

        if (isMoveWorkerMode)
        {
            TryMoveToWork(mousePosition);
            return;
        }

        TryOpenUI(mousePosition);
    }

    // Yerleşim onaylandığında işçiyi sahnede oluşturur ve envanterden düşer.
    public void HandlePlaced(Vector3 worldPos, Vector3Int cellPosition)
    {
        if (selectedWorkerData == null) return;

        Instantiate(selectedWorkerData.workerPrefab, worldPos, Quaternion.identity);

        if (inventory != null)
        {
            inventory.RemoveItem(selectedWorkerData, 1);
        }
    }

    // Tıklanan noktada işçi varsa panelini açar, yoksa açık paneli kapatır.
    public void TryOpenUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workerLayer);

        if (hit == null)
        {
            uiManager?.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        Worker worker = hit.GetComponentInParent<Worker>();
        WorkerMovement movement = hit.GetComponentInParent<WorkerMovement>();

        if (worker == null)
        {
            uiManager?.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        selectedWorkerMovement = movement;
        uiManager?.OpenWorkerUI(worker);
    }

    // Seçili envanter slotuna göre yerleştirme modunu açar veya kapatır.
    public void UpdatePlacementMode()
    {
        if (inventory == null)
            inventory = PlayerInventory.Instance;

        if (inventory == null) return;

        InventoryObject selectedObject = inventory.GetSelectedItem();

        if (!(selectedObject is WorkerData workerData))
        {
            CancelPlacementMode();
            return;
        }

        if (selectedWorkerData != workerData)
        {
            SelectWorker(workerData);
        }
    }

    // Oyuncunun şu anda bir işçiyi yerleştirme aşamasında olup olmadığını döner.
    public bool IsPlacementMode => selectedWorkerData != null;

    // Yerleştirme modunu iptal eder ve hayaleti gizler.
    public void CancelPlacementMode()
    {
        selectedWorkerData = null;
        ghostPreview?.Hide();
    }

    

    /// WORKER ÖZEL METODLARI
    /// -----------------------------------------------------------------------------------

    // İşçiyi iş alanına taşıma modunu açar veya kapatır.
    public void SetMoveWorkerMode(bool isActive)
    {
        isMoveWorkerMode = isActive;
    }

    // Tıklanan iş alanını doğrular ve seçili işçiyi oraya yönlendirir.
    private void TryMoveToWork(Vector2 mousePosition)
    {
        if (selectedWorkerMovement == null)
        {
            isMoveWorkerMode = false;
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workableLayer);
        if (hit == null)
        {
            Debug.Log("İptal: İşçi sadece belirlenen iş alanlarına gönderilebilir!");
            selectedWorkerMovement = null;
            isMoveWorkerMode = false;
            return;
        }

        Vector3Int cell = grid.WorldToCell(mousePosition);
        if (!selectedWorkerMovement.MoveTo(cell))
        {
            Debug.LogWarning("Uyarı: Seçilen hücre dolu veya ulaşılamıyor! Başka bir kare seçebilirsiniz.");
            return;
        }

        Worker worker = selectedWorkerMovement.GetComponent<Worker>();
        worker.CurrentState = WorkerState.Working;
        TransferWorkerInventoryToPlayer(worker);

        IInteractable interactable = hit.GetComponent<IInteractable>() ?? hit.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            worker.CurrentWorkType = interactable.WorkType;
        }

        Debug.Log($"İş alanına gidiliyor! Görev: {worker.CurrentWorkType}");

        selectedWorkerMovement = null;
        isMoveWorkerMode = false;
    }

    // İşçinin üzerindeki tüm eşyaları oyuncu envanterine aktarır.
    private void TransferWorkerInventoryToPlayer(Worker worker)
    {
        if (worker?.Inventory != null && PlayerInventory.Instance != null)
        {
            worker.Inventory.TransferAllToPlayer(PlayerInventory.Instance);
        }
    }


    

    // Belirtilen işçi ve eşya için taşıma rotası çizim modunu başlatır.
    public void SetTransportMode(Worker worker, ItemData item)
    {
        if (worker == null || item == null) return;

        pendingTransportWorker = worker;
        pendingTransportItem = item;
        isDrawingRoute = true;

        if (routeDrawer == null)
            routeDrawer = RouteDrawer.Instance;

        routeDrawer?.StartDrawing(OnRouteCompleted, OnRouteCancelled);
    }

    // Rota başarıyla çizildiğinde işçiye taşıma görevini atar.
    private void OnRouteCompleted(List<Vector3Int> route)
    {
        if (pendingTransportWorker != null && pendingTransportItem != null)
        {
            TransferWorkerInventoryToPlayer(pendingTransportWorker);
            pendingTransportWorker.StartTransporting(pendingTransportItem, route);
            Debug.Log($"{pendingTransportWorker.Name} taşıma görevine başladı! Taşınan: {pendingTransportItem.objectName}");
        }

        ClearTransportState();
    }

    // Rota çizimi iptal edildiğinde geçici taşıma verilerini temizler.
    private void OnRouteCancelled()
    {
        Debug.Log("Rota çizimi iptal edildi.");
        ClearTransportState();
    }

    // Taşıma ve rota çizim durumunu sıfırlar.
    private void ClearTransportState()
    {
        uiManager?.CloseAllPanels();
        pendingTransportWorker = null;
        pendingTransportItem = null;
        isDrawingRoute = false;
    }
}