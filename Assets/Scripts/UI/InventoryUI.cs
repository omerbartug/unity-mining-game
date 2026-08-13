using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform slotParent;

    private InventorySlotUI[] slotUIs = new InventorySlotUI[8];
    int lastSelectedSlotIndex;



    private void Awake()
    {
        for(int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i] = Instantiate(slotPrefab, slotParent);
            slotUIs[i].Initialize(i, inventory);
        }
    }
    private void Start()
    {
        inventory.InventoryChanged += Refresh;
        inventory.SelectedSlotChanged += SetSelectionBorder;

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
    public void SetSelectionBorder()
    {   
        int index = inventory.GetSelectedSlotIndex();

        slotUIs[lastSelectedSlotIndex].SetSelected(false);
        slotUIs[index].SetSelected(true); 

        lastSelectedSlotIndex = index;
    }

    
}