using UnityEngine;

// Oyuncunun dünya etkileşimlerini ve yakındaki işçilerle eşya transferini yönetir.
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private ProgressBar progress;
    [SerializeField] private PlayerMovement playerMovement;

    private IInteractable currentInteractable;
    private Worker currentNearbyWorker;

    private float timer = 0f;

    public static PlayerInteraction Instance { get; private set; }
    public static event System.Action OnNearbyWorkerChanged;
    public Worker CurrentNearbyWorker => currentNearbyWorker;

    // Singleton referansını kaydeder.
    private void Awake()
    {
        Instance = this;
    }

    // Etkileşim alanlarını veya menzildeki işçileri tespit eder.
    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }

        Worker worker = other.GetComponentInParent<Worker>();
        if (worker != null)
        {
            currentNearbyWorker = worker;
            OnNearbyWorkerChanged?.Invoke();
        }
    }

    // Etkileşim alanından veya işçi menzilinden çıkıldığında durumu sıfırlar.
    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable.CancelInteract(progress);
            timer = 0f;
            currentInteractable = null;
        }

        Worker worker = other.GetComponentInParent<Worker>();
        if (worker != null && worker == currentNearbyWorker)
        {
            currentNearbyWorker = null;
            OnNearbyWorkerChanged?.Invoke();
        }
    }

    // Girdi yöneticisindeki işçi etkileşim event'lerine abone olur.
    private void OnEnable()
    {
        PlayerInputManager.OnGiveToWorker += HandleGiveToWorker;
        PlayerInputManager.OnTakeFromWorker += HandleTakeFromWorker;
    }

    // Olası bellek sızıntılarını önlemek için event aboneliklerini temizler.
    private void OnDisable()
    {
        PlayerInputManager.OnGiveToWorker -= HandleGiveToWorker;
        PlayerInputManager.OnTakeFromWorker -= HandleTakeFromWorker;
    }

    // Her karede dünya etkileşim sürecini kontrol eder.
    private void Update()
    {
        HandleInteraction();
    }

    // Etkileşim tuşuna basılı tutularak yapılan zamanlı süreci (kazı, işleme, doldurma) yürütür.
    private void HandleInteraction()
    {
        if (currentInteractable != null)
        {
            bool isInteractingHeld = PlayerInputManager.Instance != null && PlayerInputManager.Instance.IsInteractingHeld;

            if (isInteractingHeld && currentInteractable.TryGetInteractionData(playerInventory, out ItemData item, out int amount))
            {
                playerMovement.DisableMovement();

                timer += Time.deltaTime;
                progress.SetProgress(timer / currentInteractable.OperationTime);

                if (timer >= currentInteractable.OperationTime)
                {
                    currentInteractable.CompleteInteract(playerInventory, item, amount);
                    timer = 0f;
                    progress.ResetProgress();
                }
            }
            else if (timer > 0f) // Tuş bırakıldıysa veya etkileşim yarıda kesildiyse iptal et
            {
                playerMovement.EnableMovement();
                currentInteractable.CancelInteract(progress);
                timer = 0f;
            }
        }
    }

    // Seçili eşyayı yakındaki işçinin girdi haznesine aktarır (F tuşu tetikleyicisi).
    private void HandleGiveToWorker()
    {
        if (currentNearbyWorker == null || playerInventory == null) return;

        InventoryObject selected = playerInventory.GetSelectedItem();
        if (selected != null)
        {
            int amount = playerInventory.GetSelectedSlot().Amount;
            int added = currentNearbyWorker.Inventory.AddToInput(selected, amount);
            if (added > 0)
            {
                playerInventory.RemoveItem(selected, added);
            }
        }
    }

    // Yakındaki işçinin girdi haznesindeki tüm eşyaları geri toplar (R tuşu tetikleyicisi).
    private void HandleTakeFromWorker()
    {
        if (currentNearbyWorker == null || playerInventory == null) return;

        if (currentNearbyWorker.Inventory.InputItems.Count > 0)
        {
            foreach (var pair in currentNearbyWorker.Inventory.InputItems)
            {
                playerInventory.AddItem(pair.Key, pair.Value);
            }
            currentNearbyWorker.Inventory.ClearInput();
        }
    }
}