using UnityEngine;

public class BuildingUIManager : MonoBehaviour
{
    [SerializeField] private MinerPanelUI minerPanel;
    [SerializeField] private ProcessorPanelUI processorPanel;

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
    }

    public void Close()
    {
        minerPanel.Close();
        processorPanel.Close();
    }
}