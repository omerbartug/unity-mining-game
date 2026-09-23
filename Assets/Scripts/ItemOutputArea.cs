using UnityEngine;

// Binaların çıkış alanıdır. Üzerine gelen Oyuncuya veya Taşıyıcı İşçiye depodaki eşyaları yükler.
public class ItemOutputArea : MonoBehaviour
{
    private IItemSource itemSource;

    private void Awake()
    {
        itemSource = GetComponentInParent<IItemSource>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 1. Oyuncu mu geldi?
        if (other.CompareTag("Player"))
        {
            Inventory playerInventory = other.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                itemSource?.CollectItems(playerInventory);
            }
            return;
        }

        // 2. Taşıyıcı işçi mi geldi?
        Worker worker = other.GetComponentInParent<Worker>();
        if (worker != null && worker.CurrentState == WorkerState.Transporting)
        {
            itemSource?.CollectItems(worker.Inventory);
        }
    }
}