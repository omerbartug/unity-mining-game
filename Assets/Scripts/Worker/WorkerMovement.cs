using UnityEngine;

public class WorkerMovement : MonoBehaviour
{
    private Worker stats;

    [SerializeField] private Grid grid;
    [SerializeField] private Vector3Int testCell;

    public bool HasReachedTarget { get; private set; }

    private Vector3 targetPosition;

    private void Awake()
    {
        stats = GetComponent<Worker>();
    }


    public void MoveTo(Vector3Int cell)
    {
        targetPosition = grid.GetCellCenterWorld(cell);
        HasReachedTarget = false;
    }

    private void Update()
    {
        if (HasReachedTarget)
            return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            stats.MovementSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            HasReachedTarget = true;
        }

    }
}