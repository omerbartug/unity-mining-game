using UnityEngine;
using UnityEngine.EventSystems;

public class WorkerManager : MonoBehaviour
{
    private WorkerMovement selectedWorkerMovement;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask oreLayer;
    [SerializeField] private WorkerUIManager uiManager;

    private bool WorkMode;
    public void SetWorkMode(bool tf)
    {
        WorkMode = tf;
    }

    public void HandleLeftClick(Vector2 mousePosition)
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (WorkMode)
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
        WorkMode = false;
    }

    private void TryOpenWorkerUI(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

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