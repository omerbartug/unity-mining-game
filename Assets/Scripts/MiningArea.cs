using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;

    
    [SerializeField] private ItemType rewardItem;
    [SerializeField] private float operationTime = 2f;
    
   

    public void Interact(Inventory inventory)
    {
        Debug.Log("Islem basladi.");
        operationTimer += Time.deltaTime;

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;

            inventory.AddItem(rewardItem, 1);

            Debug.Log("Islem bitti.");
        }
    }

    public void ResetInteract()
    {
        operationTimer = 0;
    }
}