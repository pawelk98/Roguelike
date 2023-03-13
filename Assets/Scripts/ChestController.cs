using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject chestTop;
    public bool state;
    public int coinsRangeMin;
    public int coinsRangeMax;
    int coins;

    private void Start()
    {
        coins = Random.Range(coinsRangeMin, coinsRangeMax);
    }

    public void Open()
    {
        if(!state) 
        {
            PlayerInventory.coins += coins;
            OpenChestModel();
            state = true;
            Debug.Log("Opening chest " + PlayerInventory.coins + " total coins");
        }
    }

    void OpenChestModel()
    {
        chestTop.transform.eulerAngles = chestTop.transform.eulerAngles + new Vector3(0f, 0f, -95f);
    }
}
