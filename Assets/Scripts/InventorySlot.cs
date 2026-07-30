public class InventorySlot
{
    public InventoryObject Data { get; private set; }
    public int Amount { get; private set; }

    public void Clear()
    {
        Data = null;
        Amount = 0;
    }

    public void AddAmount(int amount){
        Amount += amount;
    }
    public void RemoveAmount(int amount){
        Amount -= amount;
    }
    public void SetItem(InventoryObject data){
        Data = data;
    }
}