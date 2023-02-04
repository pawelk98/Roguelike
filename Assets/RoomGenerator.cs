using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public GameObject[] doors;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetConnections(bool[] status)
    {
        for(int i = 0; i < 4; i++)
        {
            if (status[i])
                doors[i].SetActive(false);
        }
    }
}
