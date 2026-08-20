using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{   

    [SerializeField] private Grid grid;
    [SerializeField] private NodeMaker nodes;
    [SerializeField] private PlayerInventory inventory;
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

        inventory.SelectedSlotChanged += UpdatePlacementMode;
        inventory.InventoryChanged += UpdatePlacementMode;

        Application.targetFrameRate = 120;

        inventory.AddItem(initialItem,40);
        inventory.AddItem(initialItem2,2);
    }


    private void Update()
    {

        if(!IsPlacementMode()){
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
        Collider2D blocker = Physics2D.OverlapBox(
            ghostBuilding.transform.position,
            selectedBuilding.size,
            0,
            selectedBuilding.placementBlockerLayer
        );

        Collider2D fine = Physics2D.OverlapBox(
            ghostBuilding.transform.position,
            selectedBuilding.size,
            0,
            selectedBuilding.fineLayer
        );

        canPlace = blocker == null && fine != null;
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
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
       
        if (IsPlacementMode())
        {
            TryPlaceBuilding();
            return;
        }

        TryOpenBuildingUI(mousePosition);

    }
    private void TryPlaceBuilding()
    {
        if (!canPlace)
            return;

        Instantiate(
            selectedBuilding.buildingPrefab,
            ghostBuilding.transform.position,
            Quaternion.identity
        );

        inventory.RemoveItem(selectedBuilding, 1);

        Vector3Int cellPosition = grid.WorldToCell(ghostBuilding.transform.position);
        nodes.UpdateNodeWalkability(cellPosition, false);
    }
    private void TryOpenBuildingUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, buildingLayer);

        if (hit == null)
        {
            buildingUI.Close();
            return;
        }

        Building building = hit.GetComponentInParent<Building>();

        if (building == null)
        {
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

    private bool IsPlacementMode()
    {
        return selectedBuilding != null;
    }
}