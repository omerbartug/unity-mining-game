using UnityEngine;

public class BuildingManager : MonoBehaviour
{   

    [SerializeField] private Grid grid;
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryObject initialItem;

    private BuildingData selectedBuilding;
    private GameObject ghostBuilding;
    private SpriteRenderer ghostRenderer;

    private bool canPlace;

    private readonly Color canPlaceColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    private readonly Color cantPlaceColor = new Color(1f, 0.5f, 0.5f, 0.5f);

    private void Start(){
        inventory.AddItem(initialItem,2);
    }


    private void Update()
    {

        UpdatePlacementMode();

        if(ghostBuilding == null){
            return;
        }

        MoveGhost();
        CheckPlacement();
        UpdateGhostColor();
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
        Building script = selectedBuilding.buildingPrefab.GetComponent<Building>();
        canPlace = script.CheckPlacement(ghostBuilding.transform.position, selectedBuilding.size);
    }

    private void UpdateGhostColor()
    {
        ghostRenderer.color = canPlace ? canPlaceColor : cantPlaceColor;
    }

    public void SelectBuilding(BuildingData building)
    {
        if (ghostBuilding != null)
            Destroy(ghostBuilding);

        selectedBuilding = building;

        ghostBuilding = Instantiate(selectedBuilding.ghostPrefab);

        ghostRenderer = ghostBuilding.GetComponent<SpriteRenderer>();
        ghostRenderer.color = cantPlaceColor;
    }

    public void CancelPlacementMode()
    {
        if (ghostBuilding != null)
        {
            Destroy(ghostBuilding);

            ghostBuilding = null;
            ghostRenderer = null;
        }

        selectedBuilding = null;
    }

    public void HandleLeftClick(Vector2 mousePosition)
    {
        // placement mode control
        if (selectedBuilding != null)
        {
            if (!canPlace)
                return;

            Instantiate(
                selectedBuilding.buildingPrefab,
                ghostBuilding.transform.position,
                Quaternion.identity
            );

            inventory.RemoveItem(selectedBuilding, 1);
            return;
        }

    
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider == null)
            return;

        Building building = hit.collider.GetComponent<Building>();

        if (building == null)
            return;

        building.OnSelected();
    }

    private void UpdatePlacementMode(){
        InventoryObject selectedObject = inventory.GetSelectedItem();

        if (!(selectedObject is BuildingData building))
        {
            CancelPlacementMode();
            return;
        }
        if (selectedBuilding != building)
        {
            SelectBuilding(building);
        }
    }
}