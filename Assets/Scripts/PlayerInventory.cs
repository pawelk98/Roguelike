using Mono.Cecil;
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

    [Serializable]
    public class Armor
    {
        public string name;
        public int price;
        public int pricePerLvl;
        public float armor;
        public float armorPerLvl; 
    }

    private static PlayerInventory instance;
    public static PlayerInventory Instance { get { return instance; } }
    public PlayerCombat combat;
    public Transform rightHand;
    public Transform leftHand;
    public Weapon[] weapons;
    public int[] weaponOwnership;
    public Armor armor;
    public int armorOwnership;
    public Weapon currentWeapon;
    public int maxWeaponLvl;

    GameObject currentWeaponGameObject;
    public int Coins { get; private set; }

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
        combat.damage = weapons[0].damage;
        AddCoins(Save.Instance.gameData.coins);
        weaponOwnership = Save.Instance.gameData.weaponOwnership;
        armorOwnership = Save.Instance.gameData.armorOwnership;
        SetArmor();
    }
    public string[] GetPrices()
    {
        string[] prices = new string[weapons.Length];
        int[] pricesVal = GetPricesValue();
        for (int i = 1; i < weapons.Length; i++)
        {
            if (weapons[i].price != -1)
                prices[i - 1] = pricesVal[i-1].ToString() + "g";
            else
                prices[i - 1] = "MAX";
        }
        if (armor.price != -1)
            prices[weapons.Length - 1] = pricesVal[weapons.Length - 1].ToString() + "g";
        else
            prices[weapons.Length - 1] = "MAX"; 

        return prices;
    }

    int[] GetPricesValue()
    {
        int[] prices = new int[weaponOwnership.Length + 1];

        for (int i = 0; i < weaponOwnership.Length; i++)
        {
            prices[i] = weapons[i + 1].price + weaponOwnership[i] * weapons[i + 1].pricePerLvl;
            if (weaponOwnership[i] == maxWeaponLvl)
                weapons[i + 1].price = -1;
        }
        prices[weaponOwnership.Length] = armor.price + armorOwnership * armor.pricePerLvl;
        if (armorOwnership == 6)
            armor.price = -1;
        return prices;
    }

    void SetDamage(int weaponId)
    {
        combat.damage = weapons[weaponId].damage + weapons[weaponId].damagePerLvl * (weaponOwnership[weaponId - 1] - 1);
    }

    void SetArmor()
    {
        if (armorOwnership == 0)
            combat.armor = 1;
        else
            combat.armor = 1 + armor.armor + armor.armorPerLvl * (armorOwnership - 1);
    }

    public string[] GetNames()
    {
        string[] names = new string[weapons.Length];

        for (int i = 1; i < weapons.Length; i++)
            names[i - 1] = weaponOwnership[i - 1].ToString() + " " + weapons[i].name;
        names[weapons.Length - 1] = armorOwnership.ToString() + " " + armor.name;
        return names;
    }

    public void PurchaseWeapon(int id)
    {
        int[] prices = GetPricesValue();
        if(id == weapons.Length - 1)
        {
            if (prices[id] <= Coins && armorOwnership < 6)
            {
                Coins -= prices[id];
                armorOwnership++;
                SetArmor();
                Save.Instance.SetArmorOwnership(armorOwnership);
                UIController.Instance.SetCoins(Coins);
            }
        }
        else
        {
            if (prices[id] <= Coins && weaponOwnership[id] < maxWeaponLvl)
            {
                Coins -= weapons[id + 1].price;
                weaponOwnership[id]++;
                Save.Instance.SetWeaponOwnership(id, weaponOwnership[id]);
                SwitchWeapon(id + 1);
                SetDamage(id + 1);
                UIController.Instance.SetCoins(Coins);
            }
        }
    }

    public void EquipWeapon(int id)
    {
        if (id < weaponOwnership.Length && weaponOwnership[id] > 0)
        {
            SwitchWeapon(id + 1);
            SetDamage(id + 1);
        }
    }

    public void AddCoins(int ammount)
    {
        Coins += ammount;
        UIController.Instance.SetCoins(Coins);
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