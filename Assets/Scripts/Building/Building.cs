using UnityEngine;
using System.Collections.Generic;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected BuildingData buildingData;
    protected float timer;
   
    public BuildingData Data => buildingData;
    public float Progress => timer / buildingData.productionTime;


    public abstract void CollectItems(Inventory inventory);
}