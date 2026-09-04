using UnityEngine;
using UnityEngine.EventSystems;

public class WorkerManager : MonoBehaviour
{
    private WorkerMovement selectedWorkerMovement;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask oreLayer;
    [SerializeField] private LayerMask workerLayer;
    [SerializeField] private WorkerUIManager uiManager;

    private bool MoveWorkerMode;
    public void SetMoveWorkerMode(bool tf)
    {
        MoveWorkerMode = tf;
    }

    public void HandleLeftClick(Vector2 mousePosition)
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (MoveWorkerMode)
        {
            TryMoveWorker(mousePosition);
            return;
        }

        TryOpenWorkerUI(mousePosition);

    }

    private void TryMoveWorker(Vector2 mousePosition) // controls at WorkerMovement.cs
    {
        Debug.Log("calisiyo");
        Collider2D oreHit = Physics2D.OverlapPoint(mousePosition, oreLayer);

        if (oreHit != null)
        {
                
            Vector3Int cell = grid.WorldToCell(mousePosition);
            selectedWorkerMovement.MoveTo(cell);
            Debug.Log("Madene gidiliyor!");
        }
        else
        {
            Debug.Log("İptal: İşçi sadece maden (Ore) alanlarına gönderilebilir!");
        }

        selectedWorkerMovement = null;
        MoveWorkerMode = false;
    }

    private void TryOpenWorkerUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workerLayer);

        if (hit == null)
        {
            uiManager.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        Worker worker = hit.GetComponentInParent<Worker>();
        WorkerMovement movement = hit.GetComponentInParent<WorkerMovement>();

        if (worker == null)
        {
            uiManager.CloseAllPanels();
            selectedWorkerMovement = null;
            return;
        }

        selectedWorkerMovement = movement;
        uiManager.OpenWorkerUI(worker);
    }

    
    


}