using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody playerRb;
    public float speed = 10;
    public float dodgeSpeed = 30;
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 2;

    bool dodging = false;
    float dodgeStart;
    float dodgeEnd;
    Vector3 dodgeDirection;
    void Start()
    {
 
    }

    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey("up"))
            move.z += this.speed;        
        if (Input.GetKey("down"))
            move.z -= this.speed;
        if (Input.GetKey("right"))
            move.x += this.speed;        
        if (Input.GetKey("left"))
            move.x -= this.speed;

        if (Input.GetKeyDown("z") && !dodging && Time.time - dodgeEnd >= dodgeCooldown)
        {
            dodgeStart = Time.time;
            dodgeDirection = move.normalized * dodgeSpeed;
            dodging = true;
        }

        if (dodging)
        {
            if (Time.time >= dodgeStart + dodgeDuration)
            {
                dodgeEnd = Time.time;
                dodging = false;
            }

            move = dodgeDirection;
        }

        playerRb.velocity = move;
    }

}
