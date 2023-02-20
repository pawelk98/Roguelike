using Unity.VisualScripting;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType { Start, Boss, Treasure, Default }
    public GameObject[] doors;
    public GameObject roomAssets;
    public BoxCollider boxCollider;
    public bool roomClear = false;
    RoomType roomType;
    bool[] entrances;
    

    void Awake()
    {
        entrances = new bool[4];
    }

    void Update()
    {
        if (roomClear)
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

    public void InitializeRoom(RoomType roomType)
    {
        this.roomType = roomType;
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
    }
}
