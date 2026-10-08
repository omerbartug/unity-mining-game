using System;
using UnityEngine;

// Magazanin is mantigini (Business Logic), satin alma dogrulamalarini ve islem guvenligini yoneten bilesendir.
// UI bilesenlerinden tamamen bagimsiz calisir (Single Responsibility Principle).
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private ShopCatalogueSO catalogue;

    // Disariya dinleme amacli sunulan event'ler (UI, ses ve efekt sistemleri abone olabilir)
    public event Action<ShopItemSO> OnPurchaseSuccess;
    public event Action<ShopItemSO, ShopErrorReason> OnPurchaseFailed;

    public ShopCatalogueSO Catalogue => catalogue;

    private void Awake()
    {
        // Singleton guard: sahnede birden fazla ornek varsa fazlaligi yok et, yoksa ornegi kaydet.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Bir urunun o an satin alinabilir durumda olup olmadigini dogrular.
    // UI butonunun aktif/pasif durumu ve renk geri bildirimi icin kullanilir.
    public bool CanBuy(ShopItemSO item, out ShopErrorReason? reason)
    {
        reason = null;

        if (item == null)
        {
            return false;
        }

        if (PlayerStats.Instance != null && PlayerStats.Instance.Money < item.Price)
        {
            reason = ShopErrorReason.NotEnoughMoney;
            return false;
        }

        if (PlayerInventory.Instance != null && !PlayerInventory.Instance.CanAccept(item.RewardObject, item.RewardAmount))
        {
            reason = ShopErrorReason.InventoryFull;
            return false;
        }

        // Kilitli veya ilerleme gereksinimi varsa burada kontrol edilebilir.

        return true;
    }

    // Satin alma islemini guvenli (transactional) olarak yurutur.
    public bool TryBuy(ShopItemSO item)
    {
        if (!CanBuy(item, out var reason))
        {
            if (reason.HasValue)
            {
                OnPurchaseFailed?.Invoke(item, reason.Value);
            }
            return false;
        }

        // Parayi guvenli sekilde tahsil et
        if (PlayerStats.Instance != null && !PlayerStats.Instance.TrySpendMoney(item.Price))
        {
            OnPurchaseFailed?.Invoke(item, ShopErrorReason.NotEnoughMoney);
            return false;
        }

        // Esyayi envantere ekle
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.AddItem(item.RewardObject, item.RewardAmount);
        }

        // Satin alma event'ini tetikle
        OnPurchaseSuccess?.Invoke(item);

        return true;
    }
}
