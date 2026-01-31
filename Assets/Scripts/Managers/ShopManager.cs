using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private BuildingData itemData;
    private Inventory inventory;
    private PlayerWallet wallet;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        wallet = player.GetComponent<PlayerWallet>();

        if (BuildManager.Instance != null) BuildManager.Instance.OnBuildingPlaced += OnBuildingConfirmed;
    }

    public void BuyWeapon(GameObject weaponPrefab)
    {
        //Gets weapon cost value
        Weapon weaponScript = weaponPrefab.GetComponent<Weapon>();
        int cost = weaponScript.GetPrice();

        if (wallet.Money >= cost)
        {
            wallet.SpendMoney(cost);
            inventory.AddWeapon(weaponPrefab);
        }
    }

    public void StartingBuildingPurchase(BuildingData data)
    {
        if (wallet.Money >= data.Price) BuildManager.Instance.SelectBuildingToPlace(data);
    }

    void OnBuildingConfirmed(int cost)
    {
        if (wallet.Money >= cost) wallet.SpendMoney(cost);
    }

    void OnDestroy()
    {
        if (BuildManager.Instance != null) BuildManager.Instance.OnBuildingPlaced -= OnBuildingConfirmed;
    }
}
