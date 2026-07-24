using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private int PlayerMoney = 0;

    public int getPlayerMoney(){
        return PlayerMoney;
    }
    public void setPlayerMoney(int amount){
        this.PlayerMoney += amount;
    }
}
