using UnityEngine;

public class SellingArea : MonoBehaviour, IInteractable
{
    private float operationTimer = 0f;
    private PlayerStats playerStats;


    [SerializeField] private float operationTime = 2f;

    public void Interact(Inventory inventory){
        Debug.Log("Menu Acildi");
        operationTimer += Time.deltaTime;

        if(operationTimer >= operationTime){

        }
    }

    public void ResetInteract(){
        return;
    }
    

}
