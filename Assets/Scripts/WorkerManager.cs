using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public void HandleLeftClick(Vector2 mousePosition)
{
    Collider2D hit = Physics2D.OverlapPoint(mousePosition);

    if (hit == null)
        return;

    Worker worker = hit.GetComponentInParent<Worker>();

    if (worker == null)
        return;

    Debug.Log("Worker seçildi!");
}
}