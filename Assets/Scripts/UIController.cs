using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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

    public TMP_Text health;
    public TMP_Text coins;
    public TMP_Text goalProgress;
    public TMP_Text enemies;
    public TMP_Text interactionTip;

    int enemyCount = 0;

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
                treasureImg.SetActive(true);
                break;
            case 4:
                bossImg.SetActive(true);
                break;
            case 5:
                clearedImg.SetActive(true);
                break;
            default:
                break;
        }
    }
}
