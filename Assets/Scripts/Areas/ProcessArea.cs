using UnityEngine;
public class ProcessArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;

    [SerializeField] private float operationTime = 2f;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private ProgressBar progressBar;

    public void Interact(Inventory inventory){
        ItemData selectedItem = inventory.getSelectedItem();
        if(selectedItem == null){
            return;
        }
        if(!selectedItem.processable){
            Debug.Log("islenemez");
            return;
        }
        Debug.Log("IslemBasladi");

        operationTimer += Time.deltaTime;
        progressBar.SetProgress(operationTimer,operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progressBar.ResetProgress();

            inventory.RemoveItem(selectedItem, 1);
            inventory.AddItem(selectedItem.rewardItem, 1);

            Debug.Log("islem bitti.");
        }
    }
    

    public void ResetInteract()
    {
        operationTimer = 0;
        progressBar.ResetProgress();
    }
}