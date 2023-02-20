using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody playerRb;
    public Animator animator;

    public float speed = 10;
    public float dodgeSpeed = 30;
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 2;

    float dodgeStart;
    float dodgeEnd;
    Vector3 dodgeDirection;

    bool isMoving;
    bool isDodging;

    void Start()
    {

    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey("up") || Input.GetKey("w"))
            inputDirection.z += 1;
        if (Input.GetKey("down") || Input.GetKey("s"))
            inputDirection.z -= 1;
        if (Input.GetKey("right") || Input.GetKey("d"))
            inputDirection.x += 1;
        if (Input.GetKey("left") || Input.GetKey("a"))
            inputDirection.x -= 1;

        Move(inputDirection);
        Dodge(inputDirection);
    }

    void Move(Vector3 inputDirection)
    {
        if (inputDirection.magnitude == 0)
        {
            playerRb.velocity = Vector3.zero;
            animator.SetFloat("Moving", 0);
            isMoving = false;
        }
        else if (!isDodging)
        {
            Vector3 movementDirection = inputDirection.normalized * speed;
            playerRb.velocity = movementDirection;
            transform.LookAt(playerRb.position + movementDirection);
            animator.SetFloat("Moving", movementDirection.magnitude);
            isMoving = true;
        }
    }

    void Dodge(Vector3 inputDirection)
    {
        if (Input.GetKeyDown("z") && isMoving && !isDodging && Time.time - dodgeEnd >= dodgeCooldown)
        {
            dodgeStart = Time.time;
            dodgeDirection = inputDirection.normalized * dodgeSpeed;
            animator.SetFloat("Dodging", 1 / dodgeDuration);
            isDodging = true;
        }

        if (isDodging)
        {
            playerRb.velocity = dodgeDirection;
            transform.LookAt(playerRb.position + dodgeDirection);

            if (Time.time >= dodgeStart + dodgeDuration)
            {
                dodgeEnd = Time.time;
                isDodging = false;
                animator.SetFloat("Dodging", 0);
            }
        }
    }
}
