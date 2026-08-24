using UnityEngine;
using TMPro; // TextMeshPro kullanıyorsan
using UnityEngine.UI;

public class WorkModePanelUI: MonoBehaviour
{
    [Header("UI Elemanları")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private Sprite icon;



    [SerializeField] WorkerManager workerManager;
    private Worker currentWorker;
    

    

    public void Open(Worker worker)
    {
        currentWorker = worker;
        UpdateUI();
        panel.SetActive(true);
    }

    public void Close()
    {
        currentWorker = null;
        panel.SetActive(false);
    }

    private void UpdateUI()
    {
        if (currentWorker == null) return;

        nameText.text = currentWorker.Name;
        levelText.text = $"Level: {currentWorker.Level}"; 
        speedText.text = $"Move Speed: {currentWorker.MovementSpeed}";
        capacityText.text = $"{currentWorker.StoredItemCount} / {currentWorker.CarryCapacity}";
        icon = currentWorker.CurrentItem.icon;
    }

   
    
}