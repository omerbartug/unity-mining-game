using UnityEngine;

public class WorkerUIManager : MonoBehaviour
{
    [Header("Paneller (Script Referansları)")]
    [SerializeField] private AssignTaskPanelUI assignPanel;
    [SerializeField] private WorkModePanelUI workModePanel;
    // [SerializeField] private TransportPanelUI transportPanel; // Taşıma moduna gelince bunu açarsın


    public void OpenWorkerUI(Worker worker)
    {
        CloseAllPanels();

        // İşçinin durumuna (State) bakıp, sadece ilgili panelin Open metodunu tetikliyoruz
        switch (worker.CurrentState)
        {
            case WorkerState.Idle:
                if (assignPanel != null) assignPanel.Open(worker);
                break;
                
            case WorkerState.Working:
                if (workModePanel != null) workModePanel.Open(worker);
                break;
                
            case WorkerState.Transporting:
                // if (transportPanel != null) transportPanel.Open(worker);
                Debug.Log("Taşıma paneli henüz yapılmadı!");
                break;
        }
    }

    // Hem yeni panel açılırken hem de boşa tıklandığında her şeyi kapatan temizlik metodu
    public void CloseAllPanels()
    {
        if (assignPanel != null) assignPanel.Close();
        if (workModePanel != null) workModePanel.Close();
        // if (transportPanel != null) transportPanel.Close()
    }
}