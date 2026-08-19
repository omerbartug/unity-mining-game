using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    private int PlayerMoney = 0;

    private float operationTimer;
    public float PlayerTimer => operationTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public int GetPlayerMoney(){
        return PlayerMoney;
    }
    public void AddMoney(int amount){
        PlayerMoney += amount;
    }
    public void RemoveMoney(int amount){
        PlayerMoney -= amount;
    }
    
}
