using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ShopManager : MonoBehaviour
{   
    private Inventory inventory;
    private EntityStats stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        stats = player.GetComponent<EntityStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuyWeapon(GameObject weaponPrefab)
    {
        Weapon weaponScript = weaponPrefab.GetComponent<Weapon>();

        //int cost = weaponScript.GetPrice();

        //if (playerStats.Money)
    }
}
