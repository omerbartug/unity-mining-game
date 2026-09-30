using UnityEngine;
using UnityEngine.EventSystems;

// Oyuncunun bina seçme, envanterden düşme ve bina UI etkileşimlerini yönetir.
public class BuildingManager : MonoBehaviour, IObjectInputManager
{   
    [Header("Dependencies")]
    [SerializeField] private GhostPreview ghostPreview;
    [SerializeField] private NodeMaker nodes;
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private BuildingUIManager buildingUI;
    [SerializeField] private LayerMask buildingLayer;

    private BuildingData selectedBuildingData;

    // Gerekli bileşenleri hazırlar ve hayalet önizleme bileşenini doğrular.
    private void Awake()
    {
        if (ghostPreview == null)
            ghostPreview = GetComponent<GhostPreview>() ?? gameObject.AddComponent<GhostPreview>();
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

    // Hayalet önizleme üzerinden bina yerleştirme modunu başlatır.
    public void SelectBuilding(BuildingData building)
    {
        selectedBuildingData = building;
        if (selectedBuildingData == null || ghostPreview == null) return;

        ghostPreview.Show(
            selectedBuildingData.ghostPrefab,
            selectedBuildingData.size,
            selectedBuildingData.placementBlockerLayer,
            selectedBuildingData.fineLayer
        );
    }

    /// INTERFACE METODLARI
    /// -----------------------------------------------------------------------------------

    // Fare sol tıkını yerleştirme veya bina paneli açma amacıyla işler.
    public void HandleLeftClick(Vector2 mousePosition)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (IsPlacementMode)
        {
            ghostPreview.TryConfirmPlacement();
            return;
        }

        TryOpenUI(mousePosition);
    }

    // Yerleşim onaylandığında binayı sahnede oluşturur, ızgarayı günceller ve envanterden düşer.
    public void HandlePlaced(Vector3 worldPos, Vector3Int cellPosition)
    {
        if (selectedBuildingData == null) return;

        Instantiate(selectedBuildingData.buildingPrefab, worldPos, Quaternion.identity);

        int startX = -selectedBuildingData.size.x / 2;
        int startY = -selectedBuildingData.size.y / 2;

        if (nodes != null)
        {
            for (int x = 0; x < selectedBuildingData.size.x; x++)
            {
                for (int y = 0; y < selectedBuildingData.size.y; y++)
                {
                    Vector3Int nodePos = cellPosition + new Vector3Int(startX + x, startY + y, 0);
                    nodes.UpdateNodeWalkability(nodePos, false);
                }
            }
        }

        if (inventory != null)
        {
            inventory.RemoveItem(selectedBuildingData, 1);
        }
    }

    // Tıklanan noktada bina varsa panelini açar, yoksa açık paneli kapatır.
    public void TryOpenUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, buildingLayer);

        if (hit == null)
        {
            buildingUI?.Close();
            return;
        }

        Building building = hit.GetComponentInParent<Building>();
        if (building == null)
        {
            buildingUI?.Close();
            return;
        }

        buildingUI?.Open(building);
    }

    // Seçili envanter slotuna göre yerleştirme modunu açar veya kapatır.
    public void UpdatePlacementMode()
    {
        if (inventory == null) return;

        InventoryObject selectedObject = inventory.GetSelectedItem();

        if (!(selectedObject is BuildingData building))
        {
            CancelPlacementMode();
            return;
        }

        if (selectedBuildingData != building)
        {
            SelectBuilding(building);
        }
    }

    // Oyuncunun şu anda bir binayı yerleştirme aşamasında olup olmadığını döner.
    public bool IsPlacementMode => selectedBuildingData != null;

    // Yerleştirme modunu iptal eder ve hayaleti gizler.
    public void CancelPlacementMode()
    {
        selectedBuildingData = null;
        ghostPreview?.Hide();
    }

    //------------------------------------------------------------------------------
}