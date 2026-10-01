using UnityEngine;
using TMPro;
using System.Collections.Generic;

// HUD'daki para göstergesini yönetir: anlık bakiye ve dakikadaki geliri görüntüler.
// Veri kaynakları: PlayerStats.OnMoneyChanged (bakiye), ShipmentManager.OnShipmentCompleted (gelir takibi).
public class MoneyUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI incomeText;

    [Header("Income Tracking")]
    [SerializeField] private float trackingWindowSeconds = 60f;

    private float refreshTimer;

    // Her sevkiyattan kazanılan miktarı ve zamanını kaydeden yapı.
    private struct EarningRecord
    {
        public float Time;
        public int Amount;
    }

    // Son 60 saniyedeki kazançları kronolojik sırada tutan liste (rolling window).
    private List<EarningRecord> recentEarnings = new List<EarningRecord>();

    private void OnEnable()
    {
        ShipmentManager.OnShipmentCompleted += RecordEarning; 
        
    }
    private void OnDisable()
    {

        if(PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged -= UpdateMoneyDisplay;


        ShipmentManager.OnShipmentCompleted -= RecordEarning; 

        // TODO: OnEnable'daki her iki event aboneliğini iptal et
        // TODO: Instance null kontrolü yap (sahne kapanırken Instance zaten yok olmuş olabilir)
    }

    private void Start()
    {

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnMoneyChanged += UpdateMoneyDisplay;


        UpdateMoneyDisplay(PlayerStats.Instance.Money);
        incomeText.text = "+0 / min";
        
    }

    

    // PlayerStats.OnMoneyChanged event'i tetiklendiğinde çağrılır.
    // Bakiye metnini formatlar ve günceller (örn: "Money : 1,250 $").
    private void UpdateMoneyDisplay(int currentMoney)
    {
        moneyText.text = "Money : " + currentMoney.ToString("N0");
    }

    // ShipmentManager.OnShipmentCompleted event'i tetiklendiğinde çağrılır.
    // Yeni kazancı listeye kaydeder ve gelir göstergesini günceller.
    private void RecordEarning(int amount)
    {

        if(amount <= 0)
            return;

        EarningRecord record = new EarningRecord { Time = Time.time, Amount = amount };
        recentEarnings.Add(record);
        RefreshIncomeDisplay();
    }

    // recentEarnings listesindeki süresi dolmuş kayıtları temizler (trackingWindowSeconds'tan eski olanlar).
    // Listeyi baştan tarar çünkü kayıtlar kronolojik sırada eklenir.
    private void PurgeExpiredRecords()
    {
        float cutoff = Time.time - trackingWindowSeconds;

        while (recentEarnings.Count > 0 && recentEarnings[0].Time < cutoff)
        {
            recentEarnings.RemoveAt(0);
        }
    }

    // Mevcut rolling window'daki toplam kazancı dakikalık orana çevirir ve UI'ı günceller.
    private void RefreshIncomeDisplay()
    {
        PurgeExpiredRecords();

        int totalInWindow = 0;
        for (int i = 0; i < recentEarnings.Count; i++)
        {
            totalInWindow += recentEarnings[i].Amount;
        }

        int incomePerMinute = Mathf.RoundToInt(totalInWindow * (60f / trackingWindowSeconds));
        incomeText.text = "+" + incomePerMinute.ToString("N0") + " / min";
    }
}
