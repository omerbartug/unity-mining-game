using UnityEngine;

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

    private void Awake()
    {
        Instance = this;
    }

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

    private void Update()
    {
        if (currentInteractable != null)
        {
            if (Input.GetKey(KeyCode.E) && currentInteractable.TryGetInteractionData(playerInventory, out ItemData item, out int amount))
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
            else
            {
                playerMovement.EnableMovement();
                currentInteractable.CancelInteract(progress);
                timer = 0f;
            }
        }

        // --- İŞÇİ İLE INPUT ETKİLEŞİMİ (F ve R) ---
        if (currentNearbyWorker != null && playerInventory != null)
        {
            // F: Seçili eşyayı işçinin Input'una ver
            if (Input.GetKeyDown(KeyCode.F))
            {
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

            // R: İşçinin Input'undaki tüm eşyaları geri al
            if (Input.GetKeyDown(KeyCode.R))
            {
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
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(PlayerStats.Instance.GetPlayerMoney());
        }
    }
}