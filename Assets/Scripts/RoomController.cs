using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType { Start, Boss, Treasure, Default }
    public GameObject[] doors;
    public GameObject entranceColliders;
    public GameObject roomAssets;
    RoomType roomType;

    void Start()
    {
     
    }

    void Update()
    {

    }

    public void SetConnections(bool[] status)
    {
        for (int i = 0; i < 4; i++)
        {
            if (status[i])
                doors[i].SetActive(false);
        }
    }

    public void InitializeRoom(RoomType roomType)
    {
        this.roomType = roomType;
        entranceColliders.SetActive(true);
        roomAssets.SetActive(false);
    }

    public void EnterRoom()
    {
        entranceColliders.SetActive(false);
        roomAssets.SetActive(true);
        Debug.Log("ENTERED ROOM");
    }
    
    public void ExitRoom()
    {
        entranceColliders.SetActive(true);
        roomAssets.SetActive(false);
    }
}
