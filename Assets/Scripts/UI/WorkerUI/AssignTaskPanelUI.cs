using UnityEngine;
using TMPro; // TextMeshPro için gerekli
using UnityEngine.UI; // Butonlar için gerekli

public class AssignTaskPanelUI : MonoBehaviour
{
    [Header("Sistem Referansları")]
    [SerializeField] private GameObject panel;
    [SerializeField] private WorkerManager workerManager;
    [SerializeField] private WorkerUIManager uiManager;

    [Header("UI Metinleri")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("UI Butonları")]
    [SerializeField] private Button workButton;
    [SerializeField] private Button transportButton;
    [SerializeField] private Button closeButton;

    private Worker currentWorker;

    public void Open(Worker worker)
    {
        currentWorker = worker;
        
    
        nameText.text = $"{currentWorker.Name} ({currentWorker.Level} lvl)";
        statusText.text = "Durum: Boşta (Görev Bekliyor)";

        // Buton eventlerini temizle ve yeniden bağla (eski tıklamalar üst üste binmesin diye)
        workButton.onClick.RemoveAllListeners();
        workButton.onClick.AddListener(OnWorkButtonClicked);

        transportButton.onClick.RemoveAllListeners();
        transportButton.onClick.AddListener(OnTransportButtonClicked);

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        panel.SetActive(true);
    }

    public void Close()
    {
        currentWorker = null;
        panel.SetActive(false);
    }

    private void OnWorkButtonClicked()
    {
        if (currentWorker == null) return;


        workerManager.SetWorkMode(true);
        uiManager.CloseAllPanels();
        
        Debug.Log("İş verme moduna geçildi, şimdi madene tıklanması bekleniyor...");
    }

    private void OnTransportButtonClicked()
    {
        Debug.Log("Taşıma sistemi daha yapılmadı aga, beklemede kal!");
    }

    private void OnCloseButtonClicked()
    {
        uiManager.CloseAllPanels();
    }
}