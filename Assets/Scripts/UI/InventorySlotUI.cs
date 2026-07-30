using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image selectionBorder;

    private int slotIndex;
    private InventoryUI inventoryUI;


    public void Refresh(InventorySlot slot)
    {
        if (slot.Data == null)
        {
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = slot.Data.icon;

        if (slot.Amount > 1){
            amountText.text = "x" + slot.Amount;}
        else{
            amountText.text = "";}

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        inventoryUI.SelectSlot(slotIndex);
        Debug.Log("Slota tıklandı!");
    }

    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        inventoryUI = ui;
    }

    public void setIsSelected(bool selected){
        selectionBorder.enabled = selected;
    }
    public bool isSelected(){
        return selectionBorder.enabled;
    }

}