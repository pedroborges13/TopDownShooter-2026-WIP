using JetBrains.Annotations;
using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int currentMoney;

    public int Money => currentMoney;

    //Event
    public static event Action<int> OnMoneyChanged;

    void Start()
    {
        OnMoneyChanged?.Invoke(currentMoney); //Initial value
    }
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney); //Notifies UIManager
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            OnMoneyChanged?.Invoke(currentMoney); //Notifies UIManager
            return true;
        }
        return false;
    }
}
