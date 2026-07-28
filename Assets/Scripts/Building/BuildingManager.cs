using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] private BuildingData selectedBuilding;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask placementBlockerLayer;
    [SerializeField] private LayerMask oreLayer;

    private GameObject ghostBuilding;
    private SpriteRenderer ghostRenderer;

    private bool canPlace;

    private readonly Color canPlaceColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    private readonly Color cantPlaceColor = new Color(1f, 0.5f, 0.5f, 0.5f);

    private void Start()
    {
        ghostBuilding = Instantiate(selectedBuilding.ghostPrefab);

        ghostRenderer = ghostBuilding.GetComponent<SpriteRenderer>();

        ghostRenderer.color = cantPlaceColor;
    }

    private void Update()
    {
        MoveGhost();
        CheckPlacement();
        UpdateGhostColor();

        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            Instantiate(
                selectedBuilding.buildingPrefab,
                ghostBuilding.transform.position,
                Quaternion.identity
            );
        }
    }

    private void MoveGhost()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3Int cellPosition = grid.WorldToCell(mousePos);
        ghostBuilding.transform.position = grid.GetCellCenterWorld(cellPosition);
    }

    private void CheckPlacement()
    {
        Collider2D blocker = Physics2D.OverlapBox(
            ghostBuilding.transform.position,
            (Vector2)selectedBuilding.size,
            0,
            placementBlockerLayer
        );

        Collider2D ore = Physics2D.OverlapBox(
            ghostBuilding.transform.position,
            (Vector2)selectedBuilding.size,
            0,
            oreLayer
        );

        canPlace = blocker == null && ore != null;
    }

    private void UpdateGhostColor()
    {
        ghostRenderer.color = canPlace ? canPlaceColor : cantPlaceColor;
    }
}