using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image selectionBorder;


    public void Refresh(InventorySlot slot)
    {
        if(slot.ItemType == ItemType.None || slot.Amount == 1){
            amountText.text = "";
        }
        else{
            amountText.text = "x" + slot.Amount;
        }
    }

}