using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private static UIController instance;
    public static UIController Instance { get { return instance; } }

    public GameObject wavesImg;
    public GameObject survivalImg;
    public GameObject torchesImg;
    public GameObject clearedImg;
    public GameObject treasureImg;
    public GameObject bossImg;
    public GameObject interaction;
    public GameObject weaponShopUI;
    public GameObject[] weaponItemsBg;
    public TMP_Text[] weaponItemsName;
    public TMP_Text[] weaponItemsPrice;

    public TMP_Text health;
    public TMP_Text coins;
    public TMP_Text goalProgress;
    public TMP_Text enemies;
    public TMP_Text interactionTip;

    bool isShopActive;
    int enemyCount;
    int itemSelected;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
            instance = this;
    }
    void Update()
    {
        if(isShopActive)
        {
            if (Input.GetKeyDown("up"))
                ChangeItem(false);
            if (Input.GetKeyDown("down"))
                ChangeItem(true);

            if (Input.GetKeyDown("e"))
                PlayerInventory.Instance.EquipWeapon(itemSelected);
            if (Input.GetKeyDown("b"))
            {
                PlayerInventory.Instance.PurchaseWeapon(itemSelected);
                SetWeaponNames();
                SetWeaponPrices();
            }
        }
    }

    public void SetHealth(int healthVal)
    {
        health.text = healthVal.ToString();
    }

    public void SetCoins(int coinsVal)
    { 
        coins.text = coinsVal.ToString();
    }
    
    public void AddEnemy()
    {
        enemyCount++;
        enemies.text = enemyCount.ToString();
    }
    public void RemoveEnemy()
    {
        enemyCount--;
        enemies.text = enemyCount.ToString();
    }

    public void SetGoalProgress(string text)
    {
        goalProgress.text = text;
    }

    public void SetInteractionTip(string text)
    {
        interactionTip.text = "(E) " + text;
        interaction.SetActive(true);
    }

    public void RemoveInteractionTip()
    {
        interaction.SetActive(false);
    }

    public void SetGoal(int goalId)
    {
        wavesImg.SetActive(false);
        survivalImg.SetActive(false);
        torchesImg.SetActive(false);
        clearedImg.SetActive(false);
        treasureImg.SetActive(false);
        bossImg.SetActive(false);

        switch (goalId)
        {
            case 0:
                wavesImg.SetActive(true);
                break;
            case 1:
                survivalImg.SetActive(true);
                break;
            case 2:
                torchesImg.SetActive(true);
                break;
            case 3:
                bossImg.SetActive(true);
                break;
            case 5:
                clearedImg.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OpenWeaponShop()
    {
        SetWeaponNames();
        SetWeaponPrices();
        itemSelected = 0;
        weaponItemsBg[itemSelected].SetActive(true);
        weaponShopUI.SetActive(true);
        isShopActive = true;
    }

    public void CloseWeaponShop()
    {
        weaponShopUI.SetActive(false);
        weaponItemsBg[itemSelected].SetActive(false);
        isShopActive = false;
    }

    public void ChangeItem(bool down)
    {
        weaponItemsBg[itemSelected].SetActive(false);
        if (down)
            itemSelected++;
        else
            itemSelected--;

        if (itemSelected >= weaponItemsName.Length)
            itemSelected = 0;
        else if(itemSelected < 0)
            itemSelected = weaponItemsName.Length - 1;

        weaponItemsBg[itemSelected].SetActive(true);
    }

    public bool IsShopOpened()
    {
        return isShopActive;
    }

    void SetWeaponPrices()
    {
        string[] prices = PlayerInventory.Instance.GetPrices();

        for (int i = 0; i < prices.Length; i++)
            weaponItemsPrice[i].text = prices[i];
    }

    void SetWeaponNames()
    {
        string[] names = PlayerInventory.Instance.GetNames();

        for (int i = 0; i < names.Length; i++)
            weaponItemsName[i].text = names[i].ToString();
    }
}
