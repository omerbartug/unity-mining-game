using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    private WorkerMovement selectedWorker;
    [SerializeField] private Grid grid;
    [SerializeField] private LayerMask oreLayer;

    public void HandleLeftClick(Vector2 mousePosition)
    {
        if (OrderMode())
        {
            TryMoveWorker(mousePosition);
            return;
        }

        TryPickWorker(mousePosition);
    }

    private void TryMoveWorker(Vector2 mousePosition) // controls at WorkerMovement.cs
    {
        Collider2D oreHit = Physics2D.OverlapPoint(mousePosition, oreLayer);

        if (oreHit != null)
        {
                
            Vector3Int cell = grid.WorldToCell(mousePosition);
            selectedWorker.MoveTo(cell);
            Debug.Log("Madene gidiliyor!");
        }
        else
        {
            Debug.Log("İptal: İşçi sadece maden (Ore) alanlarına gönderilebilir!");
        }

        selectedWorker = null;
    }

    private void TryPickWorker(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

        if (hit != null)
        {
            WorkerMovement worker = hit.GetComponentInParent<WorkerMovement>();

            if (worker != null)
            {
                selectedWorker = worker;
                Debug.Log("İşçi seçildi! Şimdi gideceği/çalışacağı yere tıkla.");
            }
        }
    }

    private bool OrderMode()
    {
        return selectedWorker != null;
    }
}