using UnityEngine;
using TMPro;

public class MinerPanelUI : MonoBehaviour
{
    private AutoMiner currentMiner;


    [SerializeField] private TMP_Text storageText;
    [SerializeField] private ProgressBar progressBar;

    public void Open(AutoMiner miner)
    {
        currentMiner = miner;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        currentMiner = null;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf || currentMiner == null)
            return;

        storageText.text =
            $"Storage : {currentMiner.StoredItemCount}/{currentMiner.Data.storageCapacity}";

        progressBar.SetProgress(currentMiner.Progress);
    }
}