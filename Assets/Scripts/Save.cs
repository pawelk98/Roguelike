using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save : MonoBehaviour
{
    public static Save Instance { get; private set; }
    public string saveFileName;
    [Serializable]
    public class GameData
    {
        public int coins;
        public int[] weaponOwnership;
        public int armorOwnership;
    }
    public GameData gameData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        if (File.Exists(saveFileName))
        {
            string saveFile = File.ReadAllText(saveFileName);
            gameData = JsonUtility.FromJson<GameData>(saveFile);
        }
        else
        {
            gameData = new GameData
            {
                coins = 0,
                weaponOwnership = new int[7],
                armorOwnership = 0
            };
        }
    }
    
    public void SaveAndRestart()
    {
        gameData.coins = PlayerInventory.Instance.Coins;
        string saveData = JsonUtility.ToJson(gameData);
        File.WriteAllText(saveFileName, saveData);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetWeaponOwnership(int id, int level)
    {
        gameData.weaponOwnership[id] = level;
    }

    public void SetArmorOwnership(int level)
    {
        gameData.armorOwnership = level;
    }
}
