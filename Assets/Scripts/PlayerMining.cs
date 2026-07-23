using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    private bool isProcessing = false;
    private float processTiming = 0;
    private Area currentArea;
    [SerializeField] private Inventory inventory;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        currentArea = other.GetComponent<Area>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentArea = null;
    }

    private void Update(){
        

       isProcessing = Input.GetKey(KeyCode.E) && currentArea != null;

        if(isProcessing){
            Process();
        }
        else{
            processTiming = 0f;
        }

        if(Input.GetKeyDown(KeyCode.M)){
            inventory.GetInventory();
        }
    }

    public void Process(){

        Debug.Log("islem basladi");
        processTiming += Time.deltaTime;

        if(processTiming >= currentArea.getProcessTime()){
            isProcessing = false;
            processTiming = 0f;

            if(currentArea.getConsumedItem() != ItemType.None){
                inventory.RemoveItem(currentArea.getConsumedItem(), 1);
            }
            inventory.AddItem(currentArea.getRewardItem(), 1);
            Debug.Log("islem bitti");  
            }
    }

}
