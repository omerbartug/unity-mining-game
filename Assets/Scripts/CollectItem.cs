using UnityEngine;

public class CollectItemArea : MonoBehaviour
{
    private Building building;

    private void Awake()
    {
        building = GetComponentInParent<Building>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory == null)
            return;

        building.CollectItems(inventory);
    }
}