using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    bool[] entrances;

    private void Awake()
    {
        entrances = new bool[4];
        SetObstacles();
        RotateInterior();
    }
    void Start()
    {

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if(RoomChange.currentRoom != this.gameObject)
            SetMeshRenderersState(meshRenderers, false);

        if (roomType == RoomGenerator.RoomType.Start)
            RoomClear();

        boxCollider.enabled = true;
    }

    void Update()
    {
        if (Input.GetKey("q") && RoomChange.currentRoom == gameObject)
            RoomClear();
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
            foreach (GameObject d in doors)
                d.SetActive(true);
    }
    
    public void ExitRoom()
    {
        boxCollider.enabled = true;
        SetMeshRenderersState(meshRenderers, false);
    }

    public void RoomClear()
    {
        for(int i = 0; i < 4; i++)
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

        interior.eulerAngles = new Vector3(0, degrees[Random.Range(0, 4)], 0);
    }

    void SetObstacles()
    {
        foreach (Transform t in obstacles)
        {
            if (Random.Range(0, 100) > obstacleChance)
                t.gameObject.SetActive(false);
        }
    }
}
