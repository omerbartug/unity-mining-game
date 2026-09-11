using UnityEngine;
using UnityEngine.EventSystems;

public class WorkerManager : MonoBehaviour
{
    private WorkerMovement selectedWorkerMovement;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask workableLayer;
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
            TryMoveToWork(mousePosition);
            return;
        }

        TryOpenWorkerUI(mousePosition);

    }

    private void TryMoveToWork(Vector2 mousePosition) // controls at WorkerMovement.cs
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition, workableLayer);

        if (hit != null)
        {
            Vector3Int cell = grid.WorldToCell(mousePosition);
            
            if (selectedWorkerMovement.MoveTo(cell))
            {
                Worker worker = selectedWorkerMovement.GetComponent<Worker>();
                worker.CurrentState = WorkerState.Working;

                Debug.Log("İş alanına gidiliyor!");

                selectedWorkerMovement = null;
                MoveWorkerMode = false;
            }
            else
            {
                Debug.LogWarning("Uyarı: Seçilen hücre dolu veya ulaşılamıyor! Başka bir kare seçebilirsiniz.");
            }
        }
        else
        {
            Debug.Log("İptal: İşçi sadece belirlenen iş alanlarına gönderilebilir!");
            selectedWorkerMovement = null;
            MoveWorkerMode = false;
        }
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