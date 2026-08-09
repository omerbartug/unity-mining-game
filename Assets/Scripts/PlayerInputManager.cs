using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private WorkerManager workerManager;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            workerManager.HandleLeftClick(mousePosition);
            buildingManager.HandleLeftClick(mousePosition);
        }
    }
}