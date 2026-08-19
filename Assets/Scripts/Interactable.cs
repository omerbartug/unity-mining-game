public interface IInteractable
{
    float OperationTime { get; } 
    bool TryGetInteractionData(Inventory inventory, out ItemData item, out int amount); 
    void CompleteInteract(Inventory inventory, ItemData item, int amount); 
    
    void CancelInteract(ProgressBar progress); 
}
