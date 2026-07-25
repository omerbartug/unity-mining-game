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
        if (slot.Item == null)
        {
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = slot.Item.icon;

        if (slot.Amount > 1)
            amountText.text = "x" + slot.Amount;
        else
            amountText.text = "";
    }

}