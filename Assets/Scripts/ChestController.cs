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
            PlayerInventory.Instance.AddCoins(coins);
            OpenChestModel();
            state = true;
            SoundController.Instance.PlaySound(SoundController.Instance.coinReward, transform.position, 1f);
            StartCoroutine(EndGame());
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(3);
        Save.Instance.SaveAndRestart();
    }

    void OpenChestModel()
    {
        chestTop.transform.eulerAngles = chestTop.transform.eulerAngles + new Vector3(0f, 0f, -95f);
    }
}
