using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Otomatik işleyici (AutoProcessor) binasının girdi kuyruğunu, işlenen eşyasını, çıktısını ve ilerlemesini gösteren kullanıcı arayüzüdür.
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

    // İşleyici panelini açar, event aboneliklerini kurar ve tüm alanları yeniler.
    public void Open(AutoProcessor processor)
    {
        currentProcessor = processor;

        if (currentProcessor != null)
        {
            currentProcessor.InputQueueChanged += HandleInputQueueChanged;
            currentProcessor.CurrentItemChanged += HandleCurrentItemChanged;
            currentProcessor.StorageChanged += RefreshOutput;
        }

        RefreshCurrentItem();
        RefreshInput();
        RefreshOutput();
        RefreshStatus();

        gameObject.SetActive(true);
    }

    // İşleyici panelini kapatır ve event aboneliklerini temizler.
    public void Close()
    {
        if (currentProcessor != null)
        {
            currentProcessor.InputQueueChanged -= HandleInputQueueChanged;
            currentProcessor.CurrentItemChanged -= HandleCurrentItemChanged;
            currentProcessor.StorageChanged -= RefreshOutput;
        }

        currentProcessor = null;
        gameObject.SetActive(false);
    }

    // İlerleme çubuğunu her karede işleyicinin anlık ilerleme oranına göre günceller.
    private void Update()
    {
        if (!gameObject.activeSelf || currentProcessor == null)
            return;

        RefreshProgressBar();
    }

    // Girdi kuyruğu değiştiğinde kuyruk slotlarını ve çalışma durumunu günceller.
    private void HandleInputQueueChanged()
    {
        RefreshInput();
        RefreshStatus();
    }

    // İşlenen mevcut eşya değiştiğinde ilgili kartı ve çalışma durumunu günceller.
    private void HandleCurrentItemChanged()
    {
        RefreshCurrentItem();
        RefreshStatus();
    }

    // İşleyicinin anlık çalışma durum metnini günceller.
    private void RefreshStatus()
    {
        if (statusText != null && currentProcessor != null)
        {
            statusText.text = "Status : " + currentProcessor.Status;
        }
    }

    // İlerleme çubuğunun doluluk oranını ayarlar.
    private void RefreshProgressBar()
    {
        if (progressBar != null && currentProcessor != null)
        {
            progressBar.SetProgress(currentProcessor.Progress);
        }
    }

    // O anda fırında/işleyicide işlem gören eşyanın görselini ve adını günceller.
    private void RefreshCurrentItem()
    {
        if (currentProcessor == null) return;

        if (currentProcessor.CurrentItem == null)
        {
            if (currentItemIcon != null) currentItemIcon.enabled = false;
            if (currentItemName != null) currentItemName.text = "No Item";
        }
        else
        {
            if (currentItemIcon != null)
            {
                currentItemIcon.enabled = true;
                currentItemIcon.sprite = currentProcessor.CurrentItem.icon;
            }
            if (currentItemName != null)
            {
                currentItemName.text = currentProcessor.CurrentItem.objectName;
            }
        }
    }

    // İşlenmek üzere bekleyen girdi kuyruğu slotlarını günceller.
    private void RefreshInput()
    {
        int slotIndex = DisplayQueueItems();
        ClearEmptyQueueSLots(slotIndex);
        UpdateQueueMoreText(slotIndex);
    }

    // Kuyruk eşyalarını slot ikonlarına doldurur.
    private int DisplayQueueItems()
    {
        if (currentProcessor == null || queueSlots == null) return 0;

        int slotIndex = 0;
        foreach (ItemData item in currentProcessor.InputQueue)
        {
            if (slotIndex >= queueSlots.Length)
                break;

            if (queueSlots[slotIndex] != null)
            {
                queueSlots[slotIndex].enabled = true;
                queueSlots[slotIndex].sprite = item.icon;
            }

            slotIndex++;
        }

        return slotIndex;
    }

    // Eşya bulunmayan boş kuyruk slotlarını gizler.
    private void ClearEmptyQueueSLots(int slotIndex)
    {
        if (queueSlots == null) return;

        for (int i = slotIndex; i < queueSlots.Length; i++)
        {
            if (queueSlots[i] != null)
            {
                queueSlots[i].enabled = false;
            }
        }
    }

    // Slot kapasitesinden fazla bekleyen eşya varsa fazlalık adedini (+N) gösterir.
    private void UpdateQueueMoreText(int slotIndex)
    {
        if (queueMoreText == null || currentProcessor == null) return;

        int extraCount = currentProcessor.InputQueue.Count - slotIndex;

        if (extraCount <= 0)
        {
            queueMoreText.gameObject.SetActive(false);
            return;
        }

        queueMoreText.gameObject.SetActive(true);
        queueMoreText.text = $"+{extraCount}";
    }

    // Üretilmiş ve depolanmış çıktı eşyalarını yeniler.
    private void RefreshOutput()
    {
        int slotIndex = DisplayOutputItems();
        ClearOutputSlots(slotIndex);
    }

    // Depodaki çıktı eşyalarını ikon ve miktar metinleriyle gösterir.
    private int DisplayOutputItems()
    {
        if (currentProcessor == null || outputIcons == null || outputAmounts == null) return 0;

        int slotIndex = 0;
        foreach (var pair in currentProcessor.Storage)
        {
            if (slotIndex >= outputIcons.Length)
                break;

            if (outputIcons[slotIndex] != null)
            {
                outputIcons[slotIndex].enabled = true;
                outputIcons[slotIndex].sprite = pair.Key.icon;
            }

            if (outputAmounts[slotIndex] != null)
            {
                outputAmounts[slotIndex].gameObject.SetActive(true);
                outputAmounts[slotIndex].text = $"x{pair.Value}";
            }

            slotIndex++;
        }

        return slotIndex;
    }

    // Boşta kalan çıktı slotlarını temizler ve gizler.
    private void ClearOutputSlots(int slotIndex)
    {
        if (outputIcons == null || outputAmounts == null) return;

        for (int i = slotIndex; i < outputIcons.Length; i++)
        {
            if (outputIcons[i] != null)
            {
                outputIcons[i].enabled = false;
            }

            if (outputAmounts[i] != null)
            {
                outputAmounts[i].text = "";
                outputAmounts[i].gameObject.SetActive(false);
            }
        }
    }
}
