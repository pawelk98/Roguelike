using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    public CameraMovement cameraMovement;

    public void SpawnPlayer(GameObject startingRoom)
    {
        GameObject newPlayer = Instantiate(player, startingRoom.transform.position, Quaternion.identity);
        startingRoom.GetComponent<RoomController>().EnterRoom();
        newPlayer.GetComponent<RoomChange>().SetCurrentRoom(startingRoom);

        cameraMovement.SetPlayer(newPlayer.transform);
    }
}
