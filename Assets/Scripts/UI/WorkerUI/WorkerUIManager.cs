using UnityEngine;

public class WorkerUIManager : MonoBehaviour
{
    [Header("Paneller (Script Referansları)")]
    [SerializeField] private AssignTaskPanelUI assignPanel;
    [SerializeField] private MiningWorkerPanelUI miningPanel;
    [SerializeField] private OperatingWorkerPanelUI operatingPanel;
    [SerializeField] private ProcessingWorkerPanelUI processingPanel;
    [SerializeField] private TransportingWorkerPanelUI transportPanel;

    public void OpenWorkerUI(Worker worker)
    {
        CloseAllPanels();

        switch (worker.CurrentState)
        {
            case WorkerState.Idle:
                if (assignPanel != null) assignPanel.Open(worker);
                break;
                
            case WorkerState.Working:
                switch (worker.CurrentWorkType)
                {
                    case WorkerWorkType.Mining:
                        if (miningPanel != null) miningPanel.Open(worker);
                        break;

                    case WorkerWorkType.Operating:
                        if (operatingPanel != null) operatingPanel.Open(worker);
                        break;

                    case WorkerWorkType.Processing:
                        if (processingPanel != null) processingPanel.Open(worker);
                        break;

                    default:
                        if (miningPanel != null) miningPanel.Open(worker);
                        break;
                }
                break;
                
            case WorkerState.Transporting:
                if (transportPanel != null) transportPanel.Open(worker);
                break;
        }
    }

    public void OpenTransportSetup(Worker worker)
    {
        CloseAllPanels();
        if (transportPanel != null) transportPanel.OpenForSetup(worker);
    }

    public void CloseAllPanels()
    {
        if (assignPanel != null) assignPanel.Close();
        if (miningPanel != null) miningPanel.Close();
        if (operatingPanel != null) operatingPanel.Close();
        if (processingPanel != null) processingPanel.Close();
        if (transportPanel != null) transportPanel.Close();
    }
}