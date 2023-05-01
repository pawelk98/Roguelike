using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomController : MonoBehaviour
{
    public float obstacleChance;
    public GameObject[] doors;
    public BoxCollider boxCollider;
    public bool roomClear = false;
    public RoomGenerator.RoomType roomType;
    public MeshRenderer[] meshRenderers;
    public Transform interior;
    public Transform obstacles;
    public Transform spawners;
    public Transform torches;
    public GameObject[] enemies;
    public GameObject torch;
    public int wavesRangeMin;
    public int wavesRangeMax;
    public int torchesCountRangeMin;
    public int torchesCountRangeMax;
    public float timeBetweenSpawn;
    public float survivalTimeRangeMin;
    public float survivalTimeRangeMax;
    public int minConcurrentEnemies;
    public int maxConcurrentEnemies;

    bool[] entrances;
    int roomGoal;
    bool isGoalActive;
    List<GameObject> aliveEnemies;
    float lastSpawnTime = -100;
    int wavesCount;
    int currentWave = 0;
    int concurrentEnemies;
    float roomEnterTime;
    float survivalTime;
    List<GameObject> generatedTorches;
    int torchesCount;
    List<Transform> spawnerPos;
    List<Transform> torchPos;


    private void Awake()
    {
        entrances = new bool[4];
        SetObstacles();
        RotateInterior();
    }
    void Start()
    {
        if (roomType == RoomGenerator.RoomType.Start)
            RoomClear();

        boxCollider.enabled = true;

        if (roomType == RoomGenerator.RoomType.Default)
        {
            roomGoal = UnityEngine.Random.Range(0, 3);
        }

        spawnerPos = new List<Transform>();
        torchPos = new List<Transform>();

        if (spawners != null && torches != null)
        {
            foreach (Transform t in spawners)
                if (t != spawners)
                    spawnerPos.Add(t);

            foreach (Transform t in torches)
                if (t != torches)
                    torchPos.Add(t);
        }

        aliveEnemies = new List<GameObject>();
        wavesCount = UnityEngine.Random.Range(wavesRangeMin, wavesRangeMax + 1);
        concurrentEnemies = UnityEngine.Random.Range(minConcurrentEnemies, maxConcurrentEnemies + 1);
        survivalTime = UnityEngine.Random.Range(survivalTimeRangeMin, survivalTimeRangeMax);
        generatedTorches= new List<GameObject>();
        torchesCount = UnityEngine.Random.Range(torchesCountRangeMin, torchesCountRangeMax + 1);
        concurrentEnemies = UnityEngine.Random.Range(minConcurrentEnemies, maxConcurrentEnemies + 1);

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
        if (Input.GetKey("q") && RoomChange.currentRoom == gameObject)
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
                default:
                    WavesGoalScenario();
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
            if (roomType == RoomGenerator.RoomType.Default)
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
            aliveEnemies.Remove(enemy);
    }

    void WavesGoalScenario()
    {
        if (currentWave < wavesCount && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress((currentWave + 1).ToString() + " / " + wavesCount.ToString());

            for (int i = 0; i < concurrentEnemies; i++)
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

            if (Time.time - lastSpawnTime >= timeBetweenSpawn)
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
            if (aliveEnemies.Count < concurrentEnemies)
                SpawnEnemy();
        }
        else if (!roomClear && aliveEnemies.Count == 0)
        {
            UIController.Instance.SetGoalProgress("");
            UIController.Instance.SetGoal(5);
            RoomClear();
        }
    }

    void SpawnEnemy(bool alert = false)
    {
        GameObject instantiatedEnemy = Instantiate(enemies[UnityEngine.Random.Range(0,enemies.Length)], spawnerPos[UnityEngine.Random.Range(0, spawnerPos.Count)]);
        instantiatedEnemy.GetComponent<NavMeshAgent>().enabled = true;

        if (alert)
            instantiatedEnemy.GetComponent<EnemyController>().Alert();

        aliveEnemies.Add(instantiatedEnemy);
    }

    public void KillAllEnemies()
    {
        foreach(GameObject enemy in aliveEnemies) {
            enemy.GetComponent<EnemyController>().DealDamage(1000);
        }
    }
}
