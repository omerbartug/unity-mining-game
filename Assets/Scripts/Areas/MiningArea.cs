using UnityEngine;
public class MiningArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0;

    
    [SerializeField] private ItemData rewardItem;
    [SerializeField] private float operationTime = 2f;
    [SerializeField] private ProgressBar progressBar;
   

    public void Interact(Inventory inventory)
    {
        Debug.Log("Islem basladi.");
        operationTimer += Time.deltaTime;
        progressBar.SetProgress(operationTimer,operationTime);

        if (operationTimer >= operationTime)
        {
            operationTimer = 0;
            progressBar.ResetProgress();

            inventory.AddItem(rewardItem, 1);

            Debug.Log("Islem bitti.");
        }
    }

    public void ResetInteract()
    {
        operationTimer = 0;
        progressBar.ResetProgress();
    }
}