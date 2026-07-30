using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float operationTime = 2f;
    [SerializeField] private ProgressBar progressBar;

    public void Interact(Inventory inventory){

        InventoryObject selectedItem = inventory.GetSelectedItem();
        int selectedItemAmount = inventory.GetSelectedSlot().Amount;
        if(selectedItem == null){
            return;
        }

        if(selectedItem is ItemData item){
            if(!item.sellable){
                Debug.Log("Bu urun satilamaz.");
                return;
            }
            Debug.Log("SatisBasladi");
            
            operationTimer += Time.deltaTime;
            progressBar.SetProgress(operationTimer,operationTime);

            if (operationTimer >= operationTime)
            {
                operationTimer = 0;
                progressBar.ResetProgress();

                PlayerStats.Instance.addMoney(item.sellPrice * selectedItemAmount);
                inventory.RemoveAll(item);

                Debug.Log("satis bitti.");
            }
        }
    }

    public void ResetInteract(){
        operationTimer = 0f;
        progressBar.ResetProgress();
    }
    

}
