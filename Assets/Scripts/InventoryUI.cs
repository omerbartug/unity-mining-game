using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotParent;

    private InventorySlotUI[] slotUIs = new InventorySlotUI[8];

    private void Awake()
    {
        for(int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i] = Instantiate(slotPrefab, slotParent);
        }
    }
    private void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        InventorySlot[] slots = inventory.GetSlots();

        for(int i = 0; i < slots.Length; i++)
        {
            slotUIs[i].Refresh(slots[i]);
        }
    }
}