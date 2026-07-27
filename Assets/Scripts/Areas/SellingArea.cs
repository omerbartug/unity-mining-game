using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;
    [SerializeField] private Inventory inventory;
    [SerializeField] private float operationTime = 2f;
    [SerializeField] private ProgressBar progressBar;

    public void Interact(Inventory inventory){

        ItemData selectedItem = inventory.getSelectedItem();
        int selectedItemAmount = inventory.getSelectedSlot().Amount;
        if(selectedItem == null){
            return;
        }
        if(!selectedItem.sellable){
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

            PlayerStats.Instance.addMoney(selectedItem.sellPrice * selectedItemAmount);
            inventory.RemoveAll(selectedItem);

            Debug.Log("satis bitti.");
        }
    }

    public void ResetInteract(){
        operationTimer = 0f;
        progressBar.ResetProgress();
    }
    

}
