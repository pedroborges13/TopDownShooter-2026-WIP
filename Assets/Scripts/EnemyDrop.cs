using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private int moneyReward;

    public void DropReward()
    {
        PlayerWallet wallet = FindAnyObjectByType<PlayerWallet>();

        if (wallet != null) wallet.AddMoney(moneyReward);
    }

}
