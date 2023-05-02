using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableScript : MonoBehaviour
{
    public int coinsRangeMin;
    public int coinsRangeMax;

    public void Break()
    {
        int coins = Random.Range(coinsRangeMin, coinsRangeMax + 1);
        PlayerInventory.Instance.AddCoins(coins);
        Debug.Log(coins.ToString());
        gameObject.SetActive(false);
    }
}
