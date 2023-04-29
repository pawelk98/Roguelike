using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject chestTop;
    public bool state;
    public int coinsRangeMin;
    public int coinsRangeMax;
    public int spawnChance;
    int coins;

    private void Start()
    {
        if(Random.Range(0,100) <= spawnChance)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);

        coins = Random.Range(coinsRangeMin, coinsRangeMax);
    }

    public void Open()
    {
        if(!state) 
        {
            PlayerInventory.coins += coins;
            OpenChestModel();
            state = true;
            UIController.Instance.SetCoins(PlayerInventory.coins);
        }
    }

    void OpenChestModel()
    {
        chestTop.transform.eulerAngles = chestTop.transform.eulerAngles + new Vector3(0f, 0f, -95f);
    }
}
