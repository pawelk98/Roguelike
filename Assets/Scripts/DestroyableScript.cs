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
        SoundController.Instance.PlayRandomSound(SoundController.Instance.breakVase, transform.position, 0.6f);
        gameObject.SetActive(false);
    }
}
