using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;

    [SerializeField] private float operationTime = 2f;

    public void Interact(Inventory inventory, ProgressBar progress){

        InventoryObject selectedItem = inventory.GetSelectedItem();

        if(selectedItem == null){
            return;
        }

        if(selectedItem is ItemData item){
            if(!item.sellable){
                Debug.Log("Bu urun satilamaz.");
                return;
            }
            
            Sell(inventory, item, progress);
        }
    }

    public void ResetInteract(ProgressBar progress){
        operationTimer = 0f;
        progress.ResetProgress();
    }

    private void Sell(Inventory inventory, ItemData item, ProgressBar progress){

        int selectedItemAmount = inventory.GetSelectedSlot().Amount;

        operationTimer += Time.deltaTime;
        progress.SetProgress(operationTimer / operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progress.ResetProgress();

            PlayerStats.Instance.AddMoney(item.sellPrice * selectedItemAmount);
            inventory.RemoveAll(item);

        }
    }
    

}
