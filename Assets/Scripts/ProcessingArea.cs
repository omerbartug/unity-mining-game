using UnityEngine;

public class ProcessingArea : MonoBehaviour
{
    [SerializeField] private ItemType rewardItem;

    [SerializeField] private float processingTime = 2f;

    public float getProcessingTime(){
        return processingTime;
    }
    public ItemType getRewardItem(){
        return rewardItem;
    }
}
