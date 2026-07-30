using System;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private BuildingManager buildingManager;

    private void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        buildingManager.HandleLeftClick(mousePosition);
    }
}
}