using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipmentManager : MonoBehaviour
{
    public static ShipmentManager Instance { get; private set; }

    [SerializeField] private float shipmentInterval = 30f;
    public float ShipmentInterval => shipmentInterval;

    private float currentTimer;
    public float CurrentTimer => currentTimer;

    private static readonly List<CargoContainer> activeContainers = new List<CargoContainer>();

    public static event Action<int> OnShipmentCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentTimer = shipmentInterval;
    }

    private void Update()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0f)
        {
            currentTimer = shipmentInterval;
            ExecuteShipment();
        }
    }

    public static void Register(CargoContainer container)
    {
        if (container != null && !activeContainers.Contains(container))
        {
            activeContainers.Add(container);
        }
    }

    public static void Unregister(CargoContainer container)
    {
        if (container != null)
        {
            activeContainers.Remove(container);
        }
    }

    public void ExecuteShipment()
    {
        int totalEarnings = 0;

        for (int i = activeContainers.Count - 1; i >= 0; i--)
        {
            CargoContainer container = activeContainers[i];
            if (container != null)
            {
                totalEarnings += container.SellAndClearAll();
            }
        }

        if (totalEarnings > 0)
        {
            PlayerStats.Instance.AddMoney(totalEarnings);
            Debug.Log($"[ShipmentManager] Gemi geldi! {totalEarnings}$ kazanıldı.");
        }
        else
        {
            Debug.Log("[ShipmentManager] Gemi geldi fakat konteynırlarda satılacak ürün yoktu.");
        }

        OnShipmentCompleted?.Invoke(totalEarnings);
    }
}
