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

        currentProcessor.QueueChanged += RefreshInput;
        currentProcessor.CurrentItemChanged += RefreshCurrentItem;
        currentProcessor.OutputChanged += RefreshOutput;

        RefreshCurrentItem();
        RefreshInput();
        RefreshOutput();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        if (currentProcessor != null)
        {
            currentProcessor.QueueChanged -= RefreshInput;
            currentProcessor.CurrentItemChanged -= RefreshCurrentItem;
            currentProcessor.OutputChanged -= RefreshOutput;
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
        int index = 0;

        foreach (ItemData item in currentProcessor.InputQueue)
        {
            if (index >= queueSlots.Length)
                break;

            queueSlots[index].enabled = true;
            queueSlots[index].sprite = item.icon;

            index++;
        }

        while (index < queueSlots.Length)
        {
            queueSlots[index].enabled = false;
            index++;
        }

        int extra = currentProcessor.InputQueue.Count - queueSlots.Length;

        if (extra > 0)
        {
            queueMoreText.gameObject.SetActive(true);
            queueMoreText.text = $"+{extra}";
        }
        else
        {
            queueMoreText.gameObject.SetActive(false);
        }
    }

    private void RefreshOutput()
    {
        int index = 0;

        foreach (var pair in currentProcessor.Storage)
        {
            if (index >= outputIcons.Length)
                break;

            outputIcons[index].enabled = true;
            outputIcons[index].sprite = pair.Key.icon;

            outputAmounts[index].gameObject.SetActive(true);
            outputAmounts[index].text = $"x{pair.Value}";

            index++;
        }

        
        while (index < outputIcons.Length)
        {
            outputIcons[index].enabled = false;

            outputAmounts[index].text = "";
            outputAmounts[index].gameObject.SetActive(false);

            index++;
        }
    }
}
