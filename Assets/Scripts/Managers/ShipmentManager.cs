using System;
using System.Collections.Generic;
using UnityEngine;

// Belirli aralıklarla kargo konteynerlerindeki ürünleri toptan satıp parayı oyuncuya aktaran zamanlayıcıdır.
public class ShipmentManager : MonoBehaviour
{
    public static ShipmentManager Instance { get; private set; }

    [SerializeField] private float shipmentInterval = 30f;
    public float ShipmentInterval => shipmentInterval;

    private float currentTimer;
    public float CurrentTimer => currentTimer;

    private static readonly List<CargoContainer> activeContainers = new List<CargoContainer>();

    public static event Action<int> OnShipmentCompleted;

    // Singleton örneğini kaydeder ve sevkiyat zamanlayıcısını başlatır.
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

    // Sahne geçişlerinde bellek sızıntısını önlemek için statik listeyi ve referansı temizler.
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            activeContainers.Clear();
        }
    }

    // Her karede sevkiyat süresini geri sayar ve süre dolduğunda sevkiyatı tetikler.
    private void Update()
    {
        currentTimer -= Time.deltaTime;

        if (currentTimer <= 0f)
        {
            currentTimer = shipmentInterval;
            ExecuteShipment();
        }
    }

    // Yeni oluşturulan veya aktifleşen konteyneri sevkiyat listesine kaydeder.
    public static void Register(CargoContainer container)
    {
        if (container != null && !activeContainers.Contains(container))
        {
            activeContainers.Add(container);
        }
    }

    // Devre dışı kalan veya silinen konteyneri sevkiyat listesinden çıkarır.
    public static void Unregister(CargoContainer container)
    {
        if (container != null)
        {
            activeContainers.Remove(container);
        }
    }

    // Tüm aktif konteynerlerdeki ürünleri satar ve kazanılan parayı oyuncuya aktarır.
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
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddMoney(totalEarnings);
            }
            Debug.Log($"[ShipmentManager] Gemi geldi! {totalEarnings}$ kazanıldı.");
        }
        else
        {
            Debug.Log("[ShipmentManager] Gemi geldi fakat konteynırlarda satılacak ürün yoktu.");
        }

        OnShipmentCompleted?.Invoke(totalEarnings);
    }
}
