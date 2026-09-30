using UnityEngine;

// Tıklanan bina türüne göre ilgili kullanıcı arayüzü panelini (Miner, Processor, Container) açıp kapatan yöneticidir.
public class BuildingUIManager : MonoBehaviour
{
    [SerializeField] private MinerPanelUI minerPanel;
    [SerializeField] private ProcessorPanelUI processorPanel;
    [SerializeField] private ContainerPanelUI containerPanel;

    // Gelen binanın türünü kontrol eder ve ilgili paneli açar.
    public void Open(Building building)
    {
        Close();

        if (building == null) return;

        if (building is AutoMiner miner)
        {
            minerPanel?.Open(miner);
        }
        else if (building is AutoProcessor processor)
        {
            processorPanel?.Open(processor);
        }
        else if (building is CargoContainer container)
        {
            containerPanel?.Open(container);
        }
    }

    // Açık olan tüm bina panellerini kapatır.
    public void Close()
    {
        minerPanel?.Close();
        processorPanel?.Close();
        containerPanel?.Close();
    }
}