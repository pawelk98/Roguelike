using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RoomController : MonoBehaviour
{
    public GameObject torch;
    public RoomGenerator.RoomType roomType;
    public GameObject[] doors;
    public GameObject finalChest;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshRenderers;
    public Transform interior;
    public Transform obstacles;
    public Transform spawners;
    public Transform torches;
    public float obstacleChance;
    public int reward;

    int roomGoal;
    int wavesCount;
    int currentWave = 0;
    int waveSize;
    int enemyCountWeighted = 0;
    int torchesEnemyCount;
    int torchesCount;
    bool[] enemyWaveSpawned = new bool[] { false, false, false };
    bool[] entrances;
    bool isGoalActive;
    bool roomClear;
    float lastSpawnTime = -100;
    float roomEnterTime;
    float survivalTime;
    string probabilities;
    List<GameObject> aliveEnemies;
    List<GameObject> generatedTorches;
    List<Transform> spawnerPos;
    List<Transform> torchPos;
    GameObject[] enemies;
    GameObject boss;
    EnemyController bossScript;

    private void Awake()
    {
        entrances = new bool[4];
        SetObstacles();
        RotateInterior();
    }
    void Start()
    {

        boxCollider.enabled = true;

        if (roomType == RoomGenerator.RoomType.Start)
        {
            roomGoal = 5;
            RoomClear();
        }
        else if (roomType == RoomGenerator.RoomType.Default)
        {
            enemies = Enemies.Instance.GetEnemies();
            probabilities = Enemies.Instance.GetEnemiesProbability();
            roomGoal = UnityEngine.Random.Range(0, 3);
        }
        else if (roomType == RoomGenerator.RoomType.Boss)
        {
            enemies = Enemies.Instance.GetEnemies();
            probabilities = Enemies.Instance.GetEnemiesProbability();
            roomGoal = 3;
        }

        spawnerPos = new List<Transform>();
        torchPos = new List<Transform>();

        if (spawners != null)
            foreach (Transform t in spawners)
                if (t != spawners)
                    spawnerPos.Add(t);

        if(torches != null)
            foreach (Transform t in torches)
                if (t != torches)
                    torchPos.Add(t);

        aliveEnemies = new List<GameObject>();
        wavesCount = UnityEngine.Random.Range(Enemies.Instance.wavesRangeMin, Enemies.Instance.wavesRangeMax + 1);
        waveSize = UnityEngine.Random.Range(Enemies.Instance.waveSizeMin, Enemies.Instance.waveSizeMax + 1);
        survivalTime = UnityEngine.Random.Range(Enemies.Instance.survivalTimeRangeMin, Enemies.Instance.survivalTimeRangeMax);
        generatedTorches= new List<GameObject>();
        torchesCount = UnityEngine.Random.Range(Enemies.Instance.torchesCountRangeMin, Enemies.Instance.torchesCountRangeMax + 1);
        waveSize = UnityEngine.Random.Range(Enemies.Instance.waveSizeMin, Enemies.Instance.waveSizeMax + 1);
        torchesEnemyCount = UnityEngine.Random.Range(Enemies.Instance.torchesEnemiesMin, Enemies.Instance.torchesEnemiesMax + 1);

        if(roomGoal == 2)
        {
            List<int> torchPlacement = new List<int>();
            for(int i = 0; i < torchesCount; i++)
            {
                int n;
                do {
                    n = UnityEngine.Random.Range(0, torchPos.Count);
                } while (torchPlacement.Contains(n));

                torchPlacement.Add(n);
                generatedTorches.Add(Instantiate(torch, torchPos[n]));
            }
        }

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (RoomChange.currentRoom != this.gameObject)
            SetMeshRenderersState(meshRenderers, false);
    }

    void Update()
    {
        if (Input.GetKey("k") && RoomChange.currentRoom == gameObject)
            KillAllEnemies();

        if (isGoalActive)
        {
            switch (roomGoal) {
                case 0:
                    WavesGoalScenario();
                    break;
                case 1:
                    SurvivalGoalScenario();
                    break;
                case 2:
                    TorchGoalScenario();
                    break;
                case 3:
                    BossRoomScenario();
                    break;
                default:
                    break;
            }
        }
    }

    public void SetConnections(bool[] status)
    {;
        for (int i = 0; i < 4; i++)
        {
            entrances[i] = status[i];
            if (status[i])
                doors[i].SetActive(false);
        }
    }

    public void EnterRoom()
    {
        boxCollider.enabled = false;
        SetMeshRenderersState(meshRenderers, true);

        if (!roomClear)
        {
            foreach (GameObject d in doors)
                d.SetActive(true);

            roomEnterTime = Time.time;
            isGoalActive = true;
            if (roomType == RoomGenerator.RoomType.Default || roomType == RoomGenerator.RoomType.Boss)
                UIController.Instance.SetGoal(roomGoal);
        }
    }

    public void ExitRoom()
    {
        boxCollider.enabled = true;
        SetMeshRenderersState(meshRenderers, false);
    }

    public void RoomClear()
    {
        for (int i = 0; i < 4; i++)
            if (entrances[i])
                doors[i].SetActive(false);
        roomClear = true;

        PlayerInventory.Instance.AddCoins(reward);
    }

    void SetMeshRenderersState(MeshRenderer[] renderers, bool state)
    {
        foreach (MeshRenderer r in renderers)
            r.enabled = state;
    }

    void RotateInterior()
    {
        float[] degrees = { 0, 90, 180, 270 };

        interior.eulerAngles = new Vector3(0, degrees[UnityEngine.Random.Range(0, 4)], 0);
    }

    void SetObstacles()
    {
        foreach (Transform t in obstacles)
        {
            if (UnityEngine.Random.Range(0, 100) > obstacleChance)
                t.gameObject.SetActive(false);
        }
    }
    public void KillEnemy(GameObject enemy)
    {
        if(aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
            enemyCountWeighted -= 1 / enemy.GetComponent<EnemyController>().quantity;
        }
    }

    void WavesGoalScenario()
    {
        if (currentWave < wavesCount && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress((currentWave + 1).ToString() + " / " + wavesCount.ToString());

            for (int i = 0; i < waveSize; i++)
                SpawnEnemy();
                
            currentWave++;
        }
        else if (!roomClear && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress("");
            UIController.Instance.SetGoal(5);
            RoomClear();
        }
    }
    
    void SurvivalGoalScenario()
    {
        if (Time.time - roomEnterTime <= survivalTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(survivalTime - Time.time + roomEnterTime);
            UIController.Instance.SetGoalProgress(timeSpan.Minutes + ":" + timeSpan.Seconds);

            if (Time.time - lastSpawnTime >= Enemies.Instance.timeBetweenSpawn)
            {
                SpawnEnemy(true);
                lastSpawnTime = Time.time;
            }
        }
        else if (!roomClear && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress("");
            UIController.Instance.SetGoal(5);
            RoomClear();
        }
    }

    void TorchGoalScenario()
    {
        bool allTorchesLit = true;
        int torchesLit = 0;
        if(!roomClear)
        {
            foreach (GameObject t in generatedTorches)
                if (!t.GetComponent<TorchController>().IsActivated())
                    allTorchesLit = false;
                else
                    torchesLit++;

            UIController.Instance.SetGoalProgress(torchesLit.ToString() + " / " + torchesCount.ToString());
        }

        if (!allTorchesLit)
        {
            if (enemyCountWeighted < torchesEnemyCount && Time.time - lastSpawnTime >= Enemies.Instance.timeBetweenSpawn)
            {
                lastSpawnTime = Time.time;
                SpawnEnemy();
            }
        }
        else if (!roomClear && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress("");
            UIController.Instance.SetGoal(5);
            RoomClear();
        }
    }

    void BossRoomScenario()
    {
        if (boss == null)
        {
            boss = Instantiate(Enemies.Instance.GetBoss(), spawners);
            boss.GetComponent<NavMeshAgent>().enabled = true;
            bossScript = boss.GetComponent<EnemyController>();
            aliveEnemies.Add(boss);
        }
        else
        {
            if (!enemyWaveSpawned[0] && bossScript.currentHealth <= bossScript.health / 4 * 3)
            {
                for (int i = 0; i < Enemies.Instance.bossWaves[0]; i++)
                    SpawnEnemy(true);
                enemyWaveSpawned[0] = true;
            }

            if (!enemyWaveSpawned[1] && bossScript.currentHealth <= bossScript.health / 4 * 2)
            {
                for (int i = 0; i < Enemies.Instance.bossWaves[1]; i++)
                    SpawnEnemy(true);
                enemyWaveSpawned[1] = true;
            }

            if (!enemyWaveSpawned[2] && bossScript.currentHealth <= bossScript.health / 4)
            {
                for (int i = 0; i < Enemies.Instance.bossWaves[2]; i++)
                    SpawnEnemy(true);
                enemyWaveSpawned[2] = true;
            }
        }
        if (aliveEnemies.Count == 0 && !roomClear)
        {
            UIController.Instance.SetGoalProgress("");
            UIController.Instance.SetGoal(5);
            RoomClear();
            finalChest.SetActive(true);
        }

        if (bossScript.currentHealth >= 0)
            UIController.Instance.SetGoalProgress(((int)(bossScript.currentHealth / bossScript.health * 100)).ToString() + "%");
        else
            UIController.Instance.SetGoalProgress("Dead");
    }

    void SpawnEnemy(bool alert = false)
    {
        int chosenEnemy = probabilities[UnityEngine.Random.Range(0, probabilities.Length)] - '0';
        int quantity = enemies[chosenEnemy].GetComponent<EnemyController>().quantity;

        for (int i = 0; i < quantity; i++)
        {   
            GameObject instantiatedEnemy = Instantiate(enemies[chosenEnemy], spawnerPos[UnityEngine.Random.Range(0, spawnerPos.Count)]);
            instantiatedEnemy.GetComponent<NavMeshAgent>().enabled = true;

            if (alert)
                instantiatedEnemy.GetComponent<EnemyController>().Alert();

            aliveEnemies.Add(instantiatedEnemy);
            enemyCountWeighted += 1 / quantity;
        }
    }

    public void KillAllEnemies()
    {
        foreach(GameObject enemy in aliveEnemies) {
            enemy.GetComponent<EnemyController>().DealDamage(1000);
        }
    }
}
