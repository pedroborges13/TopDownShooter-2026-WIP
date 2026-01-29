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

    public void BuyItems()
    {
        BuildManager.Instance.SelectBuildingToPlace(itemData);
    }
}
