using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float operationTime = 2f;

    public void Interact(Inventory inventory){

        ItemData selectedItem = inventory.getSelectedItem();
        if(selectedItem == null){
            return;
        }
        if(!selectedItem.sellable){
            Debug.Log("Bu urun satilamaz.");
            return;
        }
        Debug.Log("SatisBasladi");
        
        operationTimer += Time.deltaTime;

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;

            PlayerStats.Instance.addMoney(selectedItem.sellPrice);
            inventory.RemoveAll(selectedItem);

            Debug.Log("satis bitti.");
        }
    }

    public void ResetInteract(){
        operationTimer = 0f;
        return;
    }
    

}
