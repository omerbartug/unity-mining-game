using UnityEngine;

// Taşıyıcı işçinin temas ettiği bina ve işçilerle eşya alma/bırakma lojistiğini yönetir.
public class TransportLogic : MonoBehaviour
{
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

    // Temas edilen işçi veya ürün kaynağı ile eşya transferini yürütür.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sadece taşıma modundaki işçi lojistik tetikleyebilir
        if (transportItem == null || worker == null || worker.CurrentState != WorkerState.Transporting)
            return;

        // 1. Karşıdaki başka bir işçiyse: Taşıdığı ürünü onun girdi haznesine boşalt
        Worker otherWorker = other.GetComponentInParent<Worker>();
        if (otherWorker != null && otherWorker != worker)
        {
            // KORUMA: Karşıdaki de bir transporter ise birbirlerine item vermesinler!
            // Sadece normal işçilere (Operating, Processing vb.) input boşaltabilir.
            if (otherWorker.CurrentState != WorkerState.Transporting && otherWorker.Inventory != null)
            {
                inventory.TransferToInputOf(otherWorker.Inventory, transportItem);
            }
        }

        // 2. Karşı taraf bir ürün kaynağıysa (IItemSource): Eşyayı sırtına topla
        IItemSource source = other.GetComponentInParent<IItemSource>();
        if (source != null && (source as Worker) != worker)
        {
            // KORUMA: Karşıdaki kaynak başka bir transporter ise onun yükünü çalmasın!
            if (source is Worker srcWorker && srcWorker.CurrentState == WorkerState.Transporting)
                return;

            source.CollectItems(inventory);
        }
    }
}
