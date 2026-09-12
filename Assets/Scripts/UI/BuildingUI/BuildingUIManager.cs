using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    [SerializeField] private MinerPanelUI minerPanel;
    [SerializeField] private ProcessorPanelUI processorPanel;
    [SerializeField] private ContainerPanelUI containerPanel;

    public void Open(Building building)
    {
        Close();

        if (building is AutoMiner miner)
        {
            minerPanel.Open(miner);
        }
        else if (building is AutoProcessor processor)
        {
            processorPanel.Open(processor);
        }
        else if (building is CargoContainer container)
        {
            containerPanel.Open(container);
        }
    }

    public void Close()
    {
        if (minerPanel != null) minerPanel.Close();
        if (processorPanel != null) processorPanel.Close();
        if (containerPanel != null) containerPanel.Close();
    }
}