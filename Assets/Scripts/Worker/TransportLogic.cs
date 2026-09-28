using UnityEngine;

// Taşıyıcı işçinin temas ettiği bina ve işçilerle eşya alma/bırakma lojistiğini yönetir.
public class TransportLogic : MonoBehaviour
{
    private const int WorkerLayer = 9;
    private const int BuildingLayer = 6;

    private Worker worker;
    private WorkerInventory inventory;
    private ItemData transportItem;

    public ItemData TransportItem => transportItem;

    // Gerekli bileşen referanslarını önbelleğe alır.
    private void Awake()
    {
        worker = GetComponent<Worker>();
        inventory = GetComponent<WorkerInventory>();
    }

    // Taşınacak filtrelenmiş eşya türünü belirler.
    public void SetTransportItem(ItemData item)
    {
        transportItem = item;
    }

    // Taşınan eşya filtresini temizler.
    public void ClearTransportItem()
    {
        transportItem = null;
    }

    // Temas edilen işçi (Layer 9) veya bina (Layer 6) ile eşya transferini yürütür.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transportItem == null || worker == null || worker.CurrentState != WorkerState.Transporting)
            return;

        int layer = other.gameObject.layer;
        if (layer != WorkerLayer && layer != BuildingLayer)
        {
            if (other.transform.parent != null)
                layer = other.transform.parent.gameObject.layer;

            if (layer != WorkerLayer && layer != BuildingLayer)
                return;
        }

        if (layer == WorkerLayer)
        {
            Worker otherWorker = other.GetComponentInParent<Worker>();
            if (otherWorker != null && otherWorker != worker)
            {
                if (otherWorker.CurrentState != WorkerState.Transporting && otherWorker.Inventory != null)
                {
                    inventory.TransferToInputOf(otherWorker.Inventory, transportItem);
                }
            }
            return;
        }

        if (layer == BuildingLayer)
        {
            IItemSource source = other.GetComponentInParent<IItemSource>();
            source?.CollectItems(inventory);
        }
    }
}
