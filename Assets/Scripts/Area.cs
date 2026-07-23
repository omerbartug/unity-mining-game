using UnityEngine;



public class Area : MonoBehaviour
{

    [SerializeField] private ItemType rewardItem;
    [SerializeField] private ItemType consumedItem;

    [SerializeField] private float processTime = 2f;

    public float getProcessTime(){
        return processTime;
    }
    public ItemType getRewardItem(){
        return rewardItem;
    }
    public ItemType getConsumedItem(){
        return consumedItem;
    }


    
}
