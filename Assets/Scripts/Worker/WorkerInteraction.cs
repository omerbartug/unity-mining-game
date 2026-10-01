using UnityEngine;

// İşçinin iş alanlarıyla (IInteractable) zamanlayıcı ve ilerleme çubuğu üzerinden sürekli etkileşimini yönetir.
public class WorkerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;
    private Inventory workerInventory;
    private WorkerMovement workerMovement;
    private ProgressBar progress;
    private Worker worker;

    private float timer;
    private bool isInteracting = false; 
    public bool IsInteracting
    {
        get => isInteracting;
        private set
        {
            if (isInteracting == value) return;
            isInteracting = value;
            if (worker != null) worker.NotifyStatusChanged();
        }
    }

    // Gerekli bileşen ve arayüz referanslarını önbelleğe alır.
    private void Awake()
    {
        workerInventory = GetComponent<WorkerInventory>();
        workerMovement = GetComponent<WorkerMovement>();
        progress = GetComponentInChildren<ProgressBar>();
        worker = GetComponent<Worker>();
    }

    // İş alanının tetikleyicisine girildiğinde hedef etkileşimi kaydeder.
    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    // İş alanından çıkıldığında etkileşimi iptal eder ve referansı temizler.
    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            if (isInteracting)
            {
                CancelInteraction();
            }
            
            currentInteractable = null;
        }
    }

    // İşçinin durumunu kontrol eder ve hedefe ulaşıldığında etkileşimi yürütür.
    private void Update()
    {
        // İşçi çalışma (Working) durumunda değilse devam eden etkileşimi kes
        if (worker != null && worker.CurrentState != WorkerState.Working)
        {
            if (isInteracting) CancelInteraction();
            return;
        }

        if (currentInteractable == null) return;

        // Hedefe ulaşıldıysa ve iş alanı etkileşime uygunsa çalışmayı yürüt
        if (workerMovement != null && workerMovement.HasReachedTarget && 
            currentInteractable.TryGetInteractionData(workerInventory, out ItemData item, out int amount))
        {
            ExecuteInteraction(item, amount);
        }
        else if (isInteracting)
        {
            CancelInteraction();
        }
    }

    // İş alanındaki etkileşim süresini ilerletir ve süre dolduğunda işlemi tamamlar.
    private void ExecuteInteraction(ItemData item, int amount)
    {
        IsInteracting = true;

        timer += Time.deltaTime * worker.MiningSpeed;
        if (progress != null) progress.SetProgress(timer / currentInteractable.OperationTime);

        if (timer >= currentInteractable.OperationTime)
        {
            currentInteractable.CompleteInteract(workerInventory, item, amount);
            timer = 0f;
            if (progress != null) progress.ResetProgress();
        }
    }

    // Devam eden etkileşimi güvenle iptal eder, zamanlayıcıyı ve barı sıfırlar.
    private void CancelInteraction()
    {
        timer = 0f;
        if (progress != null) progress.ResetProgress();
        IsInteracting = false;
    }
}