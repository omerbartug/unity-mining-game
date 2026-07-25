using UnityEngine;
public class ProcessArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;


    [SerializeField] private ItemData consumedItem;
    [SerializeField] private ItemData rewardItem;
    [SerializeField] private float operationTime = 2f;

    public ItemData getConsumedItem(){
        return consumedItem;
    }

    public void Interact(Inventory inventory)
    {
        Debug.Log("Islem basladi.");
        if(!inventory.HasItem(consumedItem, 1)){
            Debug.Log("Hammaden eksik");
            return;
        }
        operationTimer += Time.deltaTime;

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;

            inventory.RemoveItem(consumedItem, 1);
            inventory.AddItem(rewardItem, 1);

            Debug.Log("Islem bitti.");
        }
    }

    public void ResetInteract()
    {
        operationTimer = 0;
    }
}