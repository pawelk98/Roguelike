using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChange : MonoBehaviour
{
    public GameObject currentRoom;
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void SetCurrentRoom(GameObject room)
    {
        currentRoom = room.GetComponent<Transform>().Find("EntranceColliders").gameObject;
        currentRoom.GetComponentInParent<RoomController>().EnterRoom();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Room")
        {
            currentRoom.GetComponentInParent<RoomController>().ExitRoom();
            other.GetComponentInParent<RoomController>().EnterRoom();
            currentRoom = other.gameObject;
        }
    }
}
