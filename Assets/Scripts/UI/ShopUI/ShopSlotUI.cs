using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Magaza vitrinindeki tek bir urun kartinin (slot) gorselini ve satin alma tiklamasini yonetir.
public class ShopSlotUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    private ShopItemSO currentItem;

    private void Awake()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    private void OnDestroy()
    {
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyClicked);
        }
    }

    // Kart uzerindeki verileri ScriptableObject'e gore doldurur.
    public void Setup(ShopItemSO item)
    {
        currentItem = item;

        if (itemIcon != null)
        {
            itemIcon.sprite = currentItem.Icon;
        }

        if (titleText != null)
        {
            titleText.text = currentItem.ItemName;
        }

        if (priceText != null)
        {
            priceText.text = $"{currentItem.Price} $";
        }

        RefreshButtonState();
    }

    // Oyuncunun parasi ve envanter durumuna gore satin alma butonunun aktifligini ve stilini gunceller.
    public void RefreshButtonState()
    {
        if (currentItem == null || buyButton == null)
        {
            return;
        }

        if (ShopManager.Instance != null && !ShopManager.Instance.CanBuy(currentItem, out var reason))
        {
            if (reason == ShopErrorReason.NotEnoughMoney || reason == ShopErrorReason.InventoryFull)
            {
                buyButton.image.color = Color.red;
            }
            else if (reason == ShopErrorReason.LockedOrUnavailable)
            {
                buyButton.image.color = Color.gray;
            }

            buyButton.interactable = false;
            return;
        }

        // Satin alma kosullari saglandiginda butonu tekrar aktif et ve rengini normale cevir
        buyButton.interactable = true;
        buyButton.image.color = Color.white;
    }

    // Satin al butonuna tiklandiginda tetiklenir.
    private void OnBuyClicked()
    {
        if (currentItem == null)
        {
            return;
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.TryBuy(currentItem);
        }
    }
}
