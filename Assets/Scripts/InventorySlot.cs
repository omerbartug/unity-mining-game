public class InventorySlot
{
    public ItemData Item { get; private set; }
    public int Amount { get; private set; }

    public void Clear()
    {
        Item = null;
        Amount = 0;
    }

    public void AddAmount(int amount){
        Amount += amount;
    }
    public void RemoveAmount(int amount){
        Amount -= amount;
    }
    public void SetItem(ItemData item){
        Item = item;
    }
}