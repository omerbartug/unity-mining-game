using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    private bool isMining = false;
    private float miningTiming = 0;
    private MiningArea currentMiningArea;
    [SerializeField] private Inventory inventory;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        currentMiningArea = other.GetComponent<MiningArea>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        currentMiningArea = null;
    }

    private void Update(){
        

       isMining = Input.GetKey(KeyCode.E) && currentMiningArea != null;

        if(isMining){
            Debug.Log("kazmaya baslandi");
            miningTiming += Time.deltaTime;
            if(miningTiming >= currentMiningArea.getMiningTime()){
                Debug.Log("kazma bitti");
                miningTiming = 0f;
                isMining = false;
                inventory.AddItem(currentMiningArea.getRewardItem(), 1);
            }
        }
    }

}
