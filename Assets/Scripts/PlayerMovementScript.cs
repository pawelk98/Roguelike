using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody playerRb;
    public float speed;
    public float maxSpeed;
    void Start()
    {
 
    }

    void Update()
    {
        Vector3 force = Vector3.zero;

        if (Input.GetKey("w"))
            force.z += speed;        
        if (Input.GetKey("s"))
            force.z -= speed;
        if (Input.GetKey("d"))
            force.x += speed;        
        if (Input.GetKey("a"))
            force.x -= speed;

        playerRb.velocity = force;
    }
}
