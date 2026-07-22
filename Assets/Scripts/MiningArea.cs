using UnityEngine;



public class MiningArea : MonoBehaviour
{

    [SerializeField] private ItemType rewardItem;

    [SerializeField] private float miningTime = 2f;

    public float getMiningTime(){
        return miningTime;
    }
    public ItemType getRewardItem(){
        return rewardItem;
    }


    
}
