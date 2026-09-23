using UnityEngine;
using System;

// Oyuncunun parasal durumunu yöneten Singleton bileşenidir.
public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [SerializeField] private int playerMoney = 0;

    public int Money => playerMoney;

    // Para miktarı değiştiğinde tetiklenir (yeni bakiye parametre olarak iletilir).
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int GetPlayerMoney() => playerMoney;

    // Oyuncuya para ekler.
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        playerMoney += amount;
        OnMoneyChanged?.Invoke(playerMoney);
    }

    // Oyuncudan para eksiltir (eksiye düşmez).
    public void RemoveMoney(int amount)
    {
        if (amount <= 0) return;
        playerMoney = Mathf.Max(0, playerMoney - amount);
        OnMoneyChanged?.Invoke(playerMoney);
    }

    // Yeterli bakiye varsa harcamayı gerçekleştirir.
    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0 || playerMoney < amount) return false;
        playerMoney -= amount;
        OnMoneyChanged?.Invoke(playerMoney);
        return true;
    }
}
