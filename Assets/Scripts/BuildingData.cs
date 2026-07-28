using UnityEngine;

[CreateAssetMenu(menuName = "Building/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;

    public GameObject buildingPrefab;
    
    public GameObject ghostPrefab;

    public Vector2Int size;

    public Sprite icon;

    public int price;
}