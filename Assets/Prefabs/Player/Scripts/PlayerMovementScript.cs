using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody playerRb;
    public Animator animator;
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
            move.z += 1;        
        if (Input.GetKey("down"))
            move.z -= 1;
        if (Input.GetKey("right"))
            move.x += 1;        
        if (Input.GetKey("left"))
            move.x -= 1;

        move = move.normalized * speed;
        playerRb.velocity = move;
        transform.LookAt(playerRb.position + move);
        animator.SetFloat("Moving", move.magnitude);

        if (Input.GetKeyDown("z") && !dodging && Time.time - dodgeEnd >= dodgeCooldown)
        {
            dodgeStart = Time.time;
            dodgeDirection = move.normalized * dodgeSpeed;
            animator.SetFloat("Dodging", 1 / dodgeDuration);
            dodging = true;
        }

        if(dodging)
        {
            playerRb.velocity = dodgeDirection;
            transform.LookAt(playerRb.position + dodgeDirection);

            if (Time.time >= dodgeStart + dodgeDuration)
            {
                dodgeEnd = Time.time;
                dodging = false;
                animator.SetFloat("Dodging", 0);
            }
        }
    }

}
