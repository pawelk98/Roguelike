using Unity.VisualScripting;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType { Default, Start, Boss, Treasure }
    public GameObject[] doors;
    public GameObject roomAssets;
    public BoxCollider boxCollider;
    public bool roomClear = false;
    public RoomType roomType;
    bool[] entrances;
    

    void Awake()
    {
        entrances = new bool[4];
    }

    void Start()
    {
        InitializeRoom();
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

    public void InitializeRoom()
    {
        boxCollider.enabled = true;
        roomAssets.SetActive(false);
    }

    public void EnterRoom()
    {
        boxCollider.enabled = false;
        roomAssets.SetActive(true);

        if(!roomClear)
            foreach (GameObject d in doors)
                d.SetActive(true);
    }
    
    public void ExitRoom()
    {
        boxCollider.enabled = true;
        roomAssets.SetActive(false);
    }

    public void RoomClear()
    {
        for(int i = 0; i < 4; i++)
            if (entrances[i])
                doors[i].SetActive(false);
        roomClear = true;
    }
}
