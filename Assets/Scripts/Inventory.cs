using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<GameObject> weapons = new(); //Sem o new() da NullReference
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private GameObject pistolPrefab;
    private int currentWeaponIndex;

    private int maxWeapons = 4;
    //private int maxGranades = 2;
    private GameObject currentWeapon;

    void Awake()
    {
        //Equipa a pistola
        AddWeapon(pistolPrefab);
        EquipWeapon(0);
        //Debug.Log(currentWeaponIndex);
    }
    
    void Start()
    {
        
    }

    public Weapon GetCurrentWeapon()
    {
        if (currentWeapon == null) return null;
        return currentWeapon.GetComponent<Weapon>();
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (weapons.Count > maxWeapons) return;

        GameObject newWeapon = Instantiate(weaponPrefab, weaponTransform); 
        newWeapon.gameObject.SetActive(false); //Desativa até ser equipada

        //Adiciona a lista do inventário
        weapons.Add(newWeapon);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        //Desativa a arma atual
        if (currentWeapon != null) currentWeapon.GetComponent<Weapon>().OnUnequip();

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];

        //Ativa a nova arma
        currentWeapon.GetComponent<Weapon>().OnEquip();
    }

    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(nextIndex);
    }

    public void PreviousWeapon()
    {
        //% faz o indice dar "volta" na lista, nao fica negativo
        int previousIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(previousIndex); 
    }
}
