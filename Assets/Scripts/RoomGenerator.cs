using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public enum RoomType { Start, Boss, Treasure, Default }
    public GameObject[] doors;
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
    }
}
