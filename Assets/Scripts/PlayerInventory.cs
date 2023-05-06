using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Serializable]
    public class Weapon 
    {
        public GameObject model;
        public string name;
        public int price;
        public int pricePerLvl;
        public float damage;
        public float damagePerLvl;
        public float bulletLifetime;
        public float bulletSpeed;
        public float attackSpeed;
        public float attackCooldown;
        [Range(0,1)]
        public float attackMoveSpeedMod;
        public float dodgeSpeed;
        public float dodgeDuration;
        public float dodgeCooldown;
        public int type; //0-fists, 1-onehand, 2-twohand, 3-Bow, 4-Staff
    }

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }
    public Transform rightHand;
    public Transform leftHand;
    public Weapon[] weapons;
    public int[] weaponOwnership;
    public Weapon currentWeapon;
    public int maxWeaponLvl;

    GameObject currentWeaponGameObject;
    int coins;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        currentWeapon = weapons[0];
        AddCoins(100000);
        SetOwnership();
    }

    void SetOwnership()
    {
        weaponOwnership = new int[weapons.Length - 1];
        foreach (int i in weaponOwnership)
            weaponOwnership[i] = 0;
    }
    
    public string[] GetPrices()
    {
        string[] prices = new string[weapons.Length - 1];
        for (int i = 1; i < weapons.Length; i++)
        {
            if (weapons[i].price != -1)
                prices[i - 1] = weapons[i].price.ToString() + "G";
            else
                prices[i - 1] = "MAX";
        }
        return prices;
    }

    public string[] GetNames()
    {
        string[] names = new string[weapons.Length - 1];
        for (int i = 1; i < weapons.Length; i++)
            names[i - 1] = weaponOwnership[i - 1].ToString() + " " + weapons[i].name;
        return names;
    }

    public void PurchaseWeapon(int id)
    {
        if (weapons[id + 1].price <= coins && weaponOwnership[id] < maxWeaponLvl)
        {
            coins -= weapons[id + 1].price;
            if (weaponOwnership[id] != 0)
                weapons[id + 1].damage += weapons[id + 1].damagePerLvl;

            weapons[id + 1].price += weapons[id + 1].pricePerLvl;

            if (weaponOwnership[id] == maxWeaponLvl - 1)
                weapons[id + 1].price = -1;

            weaponOwnership[id]++;
            SwitchWeapon(id + 1);
            UIController.Instance.SetCoins(coins);
        }
    }

    public void EquipWeapon(int id)
    {
        if (weaponOwnership[id] > 0)
            SwitchWeapon(id + 1);
    }

    public void AddCoins(int ammount)
    {
        coins += ammount;
        UIController.Instance.SetCoins(coins);
    }

    public void SwitchWeapon(int weaponID)
    {
        if (currentWeaponGameObject != null)
            Destroy(currentWeaponGameObject);

        if (weapons[weaponID].model != null)
        {
            Transform hand;
            if (weaponID == 5)
                hand = leftHand;
            else
                hand = rightHand;

            currentWeaponGameObject = Instantiate(weapons[weaponID].model, hand);
        }
        currentWeapon = weapons[weaponID];
    }
}