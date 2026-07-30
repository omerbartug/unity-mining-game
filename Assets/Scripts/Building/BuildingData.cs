using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building Data")]
public class BuildingData : InventoryObject
{
    [Header("General")]
    public int price;

    [Header("Prefabs")]
    public GameObject buildingPrefab;
    public GameObject ghostPrefab;

    [Header("Placement")]
    public Vector2Int size;

    [Header("Production")]
    public ItemData outputItem;
    public float productionTime;
    public int storageCapacity;
}