using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Oyuncunun tüm girdi (klavye, fare, hareket) işlemlerini tek bir merkezde toplar ve dağıtır.
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private WorkerManager workerManager;

    private Camera mainCamera;

    // Tek seferlik etkileşim event'leri
    public static event Action OnGiveToWorker;
    public static event Action OnTakeFromWorker;


    // Sürekli basılı tutulan durumlar ve eksenler
    public bool IsInteractingHeld => Input.GetKey(KeyCode.E);
    public Vector2 MovementInput { get; private set; }
    public Vector2 MouseWorldPosition { get; private set; }


    // Singleton örneğini ve ana kamera referansını kaydeder.
    private void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        
    }

    // Her karede oyuncu girdilerini toplar ve ilgili yerlere iletir.
    private void Update()
    {
        // 1. WASD / Ok tuşları hareket ekseni
        MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // 2. Farenin anlık dünya pozisyonu
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null)
            MouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 3. Rota Çizimi: RouteDrawer aktifse tüm fare hareketlerini ona yönlendir
        if (RouteDrawer.Instance != null && RouteDrawer.Instance.IsActive)
        {
            if (Input.GetMouseButtonDown(1)) // Sağ tık: Çizimi iptal et
            {
                RouteDrawer.Instance.CancelDrawing();
                return;
            }

            if (Input.GetMouseButtonDown(0)) // Sol tık basıldı: Çizimi başlat
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    return;

                RouteDrawer.Instance.HandlePointerDown(MouseWorldPosition);
                return;
            }

            if (Input.GetMouseButton(0)) // Sol tık basılı: Sürükleyerek çiz
            {
                RouteDrawer.Instance.HandlePointerDrag(MouseWorldPosition);
                return;
            }

            if (Input.GetMouseButtonUp(0)) // Sol tık bırakıldı: Çizimi tamamla
            {
                RouteDrawer.Instance.HandlePointerUp();
                return;
            }

            return; // Çizim modundayken aşağıdaki tıklamaları çalıştırma
        }

        // 4. Sol Tık: Dünya tıklaması
        if (Input.GetMouseButtonDown(0))
        {
            // UI elemanlarına (buton, panel vb.) tıklandığında dünya tıklamasını engeller.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            workerManager.HandleLeftClick(MouseWorldPosition);
            buildingManager.HandleLeftClick(MouseWorldPosition);
        }

        // 4. F Tuşu: İşçiye eşya aktarma
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnGiveToWorker?.Invoke();
        }

        // 5. R Tuşu: İşçiden eşyaları geri alma
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnTakeFromWorker?.Invoke();
        }
    }
}