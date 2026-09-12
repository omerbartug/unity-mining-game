using UnityEngine;

public abstract class Building : MonoBehaviour, IItemSource
{
    [SerializeField] protected BuildingData buildingData;
   
    public BuildingData Data => buildingData;

    public abstract void CollectItems(Inventory inventory);
}