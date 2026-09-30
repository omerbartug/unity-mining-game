using TMPro;
using UnityEngine;

// Otomatik madenci (AutoMiner) binasının depo, durum ve çalışma ilerlemesini gösteren kullanıcı arayüzüdür.
public class MinerPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text storageText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private ProgressBar progressBar;

    private AutoMiner currentMiner;

    // Madenci panelini açar, event'e abone olur ve anlık bilgileri gösterir.
    public void Open(AutoMiner miner)
    {
        currentMiner = miner;

        if (currentMiner != null)
        {
            currentMiner.OnStorageChanged += RefreshStorageAndStatus;
        }

        RefreshStorageAndStatus();
        gameObject.SetActive(true);
    }

    // Madenci panelini kapatır ve event aboneliğini temizler.
    public void Close()
    {
        if (currentMiner != null)
        {
            currentMiner.OnStorageChanged -= RefreshStorageAndStatus;
        }

        currentMiner = null;
        gameObject.SetActive(false);
    }

    // İlerleme çubuğunu her karede madencinin anlık ilerleme oranına göre günceller.
    private void Update()
    {
        if (!gameObject.activeSelf || currentMiner == null)
            return;

        if (progressBar != null)
            progressBar.SetProgress(currentMiner.Progress);
    }

    // Depo doluluk miktarını ve çalışma durumunu günceller.
    private void RefreshStorageAndStatus()
    {
        if (currentMiner == null) return;

        if (storageText != null)
        {
            storageText.text = $"Storage : {currentMiner.StoredItemCount}/{currentMiner.StorageCapacity}";
        }

        if (statusText != null)
        {
            statusText.text = $"Status   :  {currentMiner.Status}";
        }
    }
}