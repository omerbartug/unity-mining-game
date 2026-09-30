using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Oyuncu envanterindeki tek bir hücrenin (slot) simgesini, miktarını ve seçim durumunu görselleştirir.
public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Image selectionBorder;

    private int slotIndex;
    private PlayerInventory inventory;

    // Slotun indeks numarasını ve bağlı olduğu envanter referansını kaydeder.
    public void Initialize(int index, PlayerInventory inventory)
    {
        slotIndex = index;
        this.inventory = inventory;
    }

    // Slottaki eşya verisine göre simgeyi ve miktar metnini günceller.
    public void Refresh(InventorySlot slot)
    {
        if (slot == null || slot.Data == null)
        {
            icon.enabled = false;
            amountText.text = "";
            return;
        }

        icon.enabled = true;
        icon.sprite = slot.Data.icon;
        amountText.text = slot.Amount > 1 ? $"x{slot.Amount}" : "";
    }

    // Bu slotun seçili olup olmadığını belirten sarı çerçeveyi açar veya kapatır.
    public void SetSelected(bool selected)
    {
        selectionBorder.enabled = selected;
    }

    // Slota tıklandığında envanter üzerinde bu slotun seçilmesini sağlar.
    public void OnPointerClick(PointerEventData eventData)
    {
        inventory?.SelectSlot(slotIndex);
    }
}