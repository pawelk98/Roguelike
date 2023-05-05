using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Serializable]
    public class Weapon 
    {
        public GameObject model;
        public string name;
        public float damage;
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
    public Weapon currentWeapon;

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