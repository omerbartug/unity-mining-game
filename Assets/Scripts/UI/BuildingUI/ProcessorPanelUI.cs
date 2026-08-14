using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProcessorPanelUI : MonoBehaviour
{

    [Header("General")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private ProgressBar progressBar;

    [Header("Current Item")]
    [SerializeField] private Image currentItemIcon;
    [SerializeField] private TMP_Text currentItemName;

    [Header("Input Queue")]
    [SerializeField] private Image[] queueSlots;
    [SerializeField] private TMP_Text queueMoreText;

    [Header("Output")]
    [SerializeField] private Image[] outputIcons;
    [SerializeField] private TMP_Text[] outputAmounts;


    private AutoProcessor currentProcessor;

    public void Open(AutoProcessor processor)
    {
        currentProcessor = processor;

        currentProcessor.InputQueueChanged += RefreshInput;
        currentProcessor.CurrentItemChanged += RefreshCurrentItem;
        currentProcessor.StorageChanged += RefreshOutput;

        RefreshCurrentItem();
        RefreshInput();
        RefreshOutput();

        gameObject.SetActive(true);
    }
    public void Close()
    {
        if (currentProcessor != null)
        {
            currentProcessor.InputQueueChanged -= RefreshInput;
            currentProcessor.CurrentItemChanged -= RefreshCurrentItem;
            currentProcessor.StorageChanged -= RefreshOutput;
        }

        currentProcessor = null;
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (!gameObject.activeSelf || currentProcessor == null)
            return;
  
        RefreshProgressBar();

    }


    private void RefreshProgressBar(){

        progressBar.SetProgress(currentProcessor.Progress);

    }


    private void RefreshCurrentItem(){
        Debug.Log(currentProcessor.CurrentItem);

        if (currentProcessor.CurrentItem == null)
        {
            currentItemIcon.enabled = false;
            currentItemName.text = "No Item";
        }
        else
        {
            currentItemIcon.enabled = true;
            currentItemIcon.sprite = currentProcessor.CurrentItem.icon;
            currentItemName.text = currentProcessor.CurrentItem.objectName;
        }

    }


    private void RefreshInput()
    {
        int slotIndex = DisplayQueueItems();

        ClearEmptyQueueSLots(slotIndex);
        UpdateQueueMoreText(slotIndex);
    }
    private int DisplayQueueItems()
    {
        int slotIndex = 0;

        foreach (ItemData item in currentProcessor.InputQueue)
        {
            if (slotIndex >= queueSlots.Length)
                break;

            queueSlots[slotIndex].enabled = true;
            queueSlots[slotIndex].sprite = item.icon;

            slotIndex++;
        }

        return slotIndex;
    }
    private void ClearEmptyQueueSLots(int slotIndex)
    {
        for (int i = slotIndex; i < queueSlots.Length; i++)
        {
            queueSlots[i].enabled = false;
        }
    }
    private void UpdateQueueMoreText(int slotIndex)
    {
        int extraCount = currentProcessor.InputQueue.Count - slotIndex;

        if (extraCount <= 0)
        {
            queueMoreText.gameObject.SetActive(false);
            return;
        }

        queueMoreText.gameObject.SetActive(true);
        queueMoreText.text = $"+{extraCount}";
    }


    private void RefreshOutput()
    {
        int slotIndex = DisplayOutputItems();

        ClearOutputSlots(slotIndex);
    }
    private int DisplayOutputItems()
    {
        int slotIndex = 0;

        foreach (var pair in currentProcessor.Storage)
        {
            if (slotIndex >= outputIcons.Length)
                break;

            outputIcons[slotIndex].enabled = true;
            outputIcons[slotIndex].sprite = pair.Key.icon;

            outputAmounts[slotIndex].gameObject.SetActive(true);
            outputAmounts[slotIndex].text = $"x{pair.Value}";

            slotIndex++;
        }

        return slotIndex;
    }
    private void ClearOutputSlots(int slotIndex)
    {
        for (int i = slotIndex; i < outputIcons.Length; i++)
        {
            outputIcons[i].enabled = false;

            outputAmounts[i].text = "";
            outputAmounts[i].gameObject.SetActive(false);
        }
    }
}
