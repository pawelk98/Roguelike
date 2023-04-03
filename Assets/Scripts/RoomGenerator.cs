using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelGenerator;

public class RoomGenerator : MonoBehaviour
{
    public enum RoomType { Default, Start, Boss, Treasure }

    public GameObject[] roomStart;
    public GameObject[] roomDefault;
    public GameObject[] roomBoss;
    public GameObject[] roomTreasure;

    public GameObject GenerateRoom(RoomType roomType, Vector3 position, bool[] connections)
    {
        GameObject room = Instantiate(GetRoomPrefab(roomType), position, Quaternion.identity, transform);
        RoomController roomController = room.GetComponent<RoomController>();
        roomController.SetConnections(connections);
        room.name = "Room x:" + position.x + " y:" + position.z;

        return room;
    }

    GameObject GetRoomPrefab(RoomType roomType)
    {
        GameObject room;
        switch(roomType)
        {
            case RoomType.Default:
                room = roomDefault[Random.Range(0, roomDefault.Length)];
                break;
            case RoomType.Start:
                room = roomStart[Random.Range(0, roomStart.Length)];
                break;
            case RoomType.Boss:
                room = roomBoss[Random.Range(0, roomBoss.Length)];
                break;
            case RoomType.Treasure:
                room = roomTreasure[Random.Range(0, roomTreasure.Length)];
                break;
            default:
                room = roomStart[0];
                break;
        }

        return room;
    }
}
