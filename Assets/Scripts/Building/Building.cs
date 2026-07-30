using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [SerializeField] protected BuildingData buildingData;

    public abstract bool CheckPlacement(Vector2 position, Vector2 size);

    public virtual void OnSelected()
    {
        Debug.Log("Building seçildi.");
    }
}