using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemies : MonoBehaviour
{
    private static Enemies instance;
    public static Enemies Instance { get { return instance; } }

    public GameObject[] skeletons;
    public GameObject[] goblins;
    public GameObject[] spiders;
    public GameObject[] golems;
    public GameObject[] bosses;

    public int primaryToSecondaryRatio = 4;

    public int wavesRangeMin;
    public int wavesRangeMax;
    public int waveSizeMin;
    public int waveSizeMax;
    public int torchesCountRangeMin;
    public int torchesCountRangeMax;
    public int torchesEnemiesMin;
    public int torchesEnemiesMax;
    public int[] bossWaves;
    public float survivalTimeRangeMin;
    public float survivalTimeRangeMax;
    public float timeBetweenSpawn;

    GameObject[] allEnemies;
    List<GameObject[]> enemiesByType;

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
        enemiesByType = new List<GameObject[]>();
        allEnemies = new GameObject[skeletons.Length + goblins.Length + spiders.Length + golems.Length];
        for (int i = 0; i < skeletons.Length; i++)
            allEnemies[i] = skeletons[i];
        for (int i = 0; i < goblins.Length; i++)
            allEnemies[i + skeletons.Length] = goblins[i];
        for (int i = 0; i < spiders.Length; i++)
            allEnemies[i + skeletons.Length + goblins.Length] = spiders[i];        
        for (int i = 0; i < golems.Length; i++)
            allEnemies[i + skeletons.Length + goblins.Length + spiders.Length] = golems[i];
    }

    public GameObject[] GetEnemies()
    {
        return allEnemies;
    }

    public GameObject GetBoss()
    {
        return bosses[UnityEngine.Random.Range(0,bosses.Length)];
    }

    public string GetEnemiesProbability()
    {
        enemiesByType.Clear();
        enemiesByType.Add(skeletons);
        enemiesByType.Add(goblins);
        enemiesByType.Add(spiders);
        enemiesByType.Add(golems);

        string probabilities = "";

        GameObject[] primaryType = enemiesByType[UnityEngine.Random.Range(0, enemiesByType.Count)];
        enemiesByType.Remove(primaryType);
        GameObject[] secondaryType = enemiesByType[UnityEngine.Random.Range(0, enemiesByType.Count)];

        foreach (GameObject e in primaryType)
        {
            int arrayIndex = Array.IndexOf(allEnemies, e);
            int spawnChance = UnityEngine.Random.Range(1, 5);
            for (int i = 0; i < spawnChance * primaryToSecondaryRatio; i++)
                probabilities += arrayIndex.ToString();
        }

        foreach (GameObject e in secondaryType)
        {
            int arrayIndex = Array.IndexOf(allEnemies, e);
            int spawnChance = UnityEngine.Random.Range(0, 5);
            for (int i = 0; i < spawnChance; i++)
                probabilities += arrayIndex.ToString();
        }

        return probabilities;
    }
}
