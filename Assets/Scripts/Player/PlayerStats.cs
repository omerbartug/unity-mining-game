using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    public int PlayerMoney = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public int getPlayerMoney(){
        return PlayerMoney;
    }
    public void addMoney(int amount){
        PlayerMoney += amount;
    }
    public void RemoveMoney(int amount){
        PlayerMoney += amount;
    }
    
}
