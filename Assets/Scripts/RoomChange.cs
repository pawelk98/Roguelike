using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChange : MonoBehaviour
{
    public GameObject currentRoom;

    public void SetCurrentRoom(GameObject room)
    {
        currentRoom = room;
        currentRoom.GetComponent<RoomController>().EnterRoom();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Room")
        {
            currentRoom.GetComponent<RoomController>().ExitRoom();
            other.GetComponent<RoomController>().EnterRoom();
            currentRoom = other.gameObject;
        }
    }
}
