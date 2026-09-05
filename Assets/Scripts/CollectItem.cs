using UnityEngine;

public class CollectItem : MonoBehaviour
{
    private IItemSource itemSource;

    private void Awake()
    {
        itemSource = GetComponentInParent<IItemSource>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory inventory = other.GetComponent<Inventory>();

        if (inventory == null)
            return;

        itemSource?.CollectItems(inventory);
    }
}