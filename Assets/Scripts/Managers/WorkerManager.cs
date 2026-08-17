using UnityEngine;

public class WorkerManager : MonoBehaviour
{

    private Worker selectedWorker;
    [SerializeField] private Grid grid;

    public void HandleLeftClick(Vector2 mousePosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);

        if (hit != null)
        {
            Worker worker = hit.GetComponentInParent<Worker>();

            if (worker != null)
            {
                selectedWorker = worker;
                return;
            }
        }

        if (selectedWorker == null)
            return;

        Vector3Int cell = grid.WorldToCell(mousePosition);

        WorkerMovement movement = selectedWorker.GetComponent<WorkerMovement>();

        movement.MoveTo(cell);
    }
}