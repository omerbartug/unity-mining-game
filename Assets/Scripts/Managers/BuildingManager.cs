using UnityEngine;

public class BuildingManager : MonoBehaviour
{   

    [SerializeField] private Grid grid;
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventoryObject initialItem;
    [SerializeField] private InventoryObject initialItem2;
    [SerializeField] private BuildingUIManager buildingUI;
    [SerializeField] private LayerMask buildingLayer;


    private BuildingData selectedBuilding;
    private GameObject ghostBuilding;
    private SpriteRenderer ghostRenderer;

    private bool canPlace;

    private readonly Color canPlaceColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    private readonly Color cantPlaceColor = new Color(1f, 0.5f, 0.5f, 0.5f);

    private void Start(){
        inventory.AddItem(initialItem,2);
        inventory.AddItem(initialItem2,2);
    }


    private void Update()
    {

        UpdatePlacementMode();

        if(ghostBuilding == null){
            return;
        }

        MoveGhost();
        CheckPlacement(selectedBuilding.placementBlockerLayer, selectedBuilding.fineLayer);
        UpdateGhostColor();
    }

    private void MoveGhost()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3Int cellPosition = grid.WorldToCell(mousePos);
        ghostBuilding.transform.position = grid.GetCellCenterWorld(cellPosition);
    }

    private void CheckPlacement(LayerMask blocker, LayerMask fine)
    {
        Collider2D Block = Physics2D.OverlapBox(ghostBuilding.transform.position, selectedBuilding.size, 0, blocker);
        Collider2D Okey = Physics2D.OverlapBox(ghostBuilding.transform.position, selectedBuilding.size, 0, fine);

        canPlace = Block == null && Okey != null;
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

    
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, buildingLayer);
        

        if (hit == null){
            buildingUI.Close();
            return;
        }

        Building building = hit.GetComponentInParent<Building>();
        
        if (building == null){
            buildingUI.Close();
            return;
        }

        buildingUI.Open(building);
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