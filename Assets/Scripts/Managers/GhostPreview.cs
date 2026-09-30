using System;
using UnityEngine;

// Izgara tabanlı hayalet önizleme ve yerleştirme doğrulamasını yöneten bağımsız bileşendir.
public class GhostPreview : MonoBehaviour
{
    [SerializeField] private Grid grid;

    private GameObject ghostInstance;
    private SpriteRenderer ghostRenderer;

    private Vector2Int currentSize;
    private LayerMask currentBlockerLayer;
    private LayerMask currentFineLayer;

    private readonly Color canPlaceColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    private readonly Color cantPlaceColor = new Color(1f, 0.5f, 0.5f, 0.5f);

    public bool IsActive => ghostInstance != null;
    public bool CanPlace { get; private set; }
    public Vector3 CurrentWorldPosition => ghostInstance != null ? ghostInstance.transform.position : Vector3.zero;
    public Vector3Int CurrentCellPosition => (grid != null && ghostInstance != null) ? grid.WorldToCell(ghostInstance.transform.position) : Vector3Int.zero;

    // Yerleştirme onaylandığında dünya ve ızgara koordinatlarını iletir.
    public event Action<Vector3, Vector3Int> OnPlacementConfirmed;

    // Izgara referansını önbelleğe alır.
    private void Awake()
    {
        if (grid == null) grid = FindFirstObjectByType<Grid>();
    }

    // Belirtilen hayalet prefabı ve katman kurallarıyla önizlemeyi başlatır.
    public void Show(GameObject ghostPrefab, Vector2Int size, LayerMask blockerLayer, LayerMask fineLayer)
    {
        Hide();
        if (ghostPrefab == null) return;

        currentSize = size;
        currentBlockerLayer = blockerLayer;
        currentFineLayer = fineLayer;

        ghostInstance = Instantiate(ghostPrefab);
        ghostRenderer = ghostInstance.GetComponent<SpriteRenderer>();

        if (ghostRenderer != null)
        {
            ghostRenderer.color = cantPlaceColor;
        }
    }

    // Hayalet önizlemeyi sonlandırır ve sahneden siler.
    public void Hide()
    {
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
            ghostRenderer = null;
        }

        CanPlace = false;
    }

    // Dışarıdan gelen fare dünya koordinatına göre hayaleti ızgaraya hizalar ve kontrol eder.
    public void UpdatePreview(Vector2 mouseWorldPosition)
    {
        if (!IsActive || grid == null || ghostInstance == null) return;

        Vector3Int cellPosition = grid.WorldToCell(mouseWorldPosition);
        ghostInstance.transform.position = grid.GetCellCenterWorld(cellPosition);

        CheckPlacement();
        UpdateGhostColor();
    }

    // Konum uygunsa yerleşimi onaylar ve event fırlatır.
    public bool TryConfirmPlacement()
    {
        if (!IsActive || !CanPlace) return false;

        OnPlacementConfirmed?.Invoke(CurrentWorldPosition, CurrentCellPosition);
        return true;
    }

    // Alanda engel veya zorunlu zemin katmanı durumunu doğrular.
    private void CheckPlacement()
    {
        if (ghostInstance == null) return;

        Collider2D blocker = Physics2D.OverlapBox(ghostInstance.transform.position, currentSize, 0, currentBlockerLayer);

        bool fineCheckPassed = true;
        if (currentFineLayer.value != 0)
        {
            Collider2D fine = Physics2D.OverlapBox(ghostInstance.transform.position, currentSize, 0, currentFineLayer);
            fineCheckPassed = fine != null;
        }

        CanPlace = blocker == null && fineCheckPassed;
    }

    // Uygunluk durumuna göre hayaletin rengini (yeşil/kırmızı) günceller.
    private void UpdateGhostColor()
    {
        if (ghostRenderer != null)
        {
            ghostRenderer.color = CanPlace ? canPlaceColor : cantPlaceColor;
        }
    }
}
