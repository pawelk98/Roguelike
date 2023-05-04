using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody playerRb;
    public Animator animator;
    public PlayerCombat playerCombat;

    public float speed = 10;
    public float dodgeSpeed = 30;
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 2;
    public float attackSpeed = 1f;
    public float attackCooldown = 1f;
    public float attackMoveSpeedMod = 0.5f;
    public float maxAttackDirectionInputTime = 0.2f;

    float attackStart;
    float dodgeStart;
    float dodgeEnd;
    Vector3 dodgeDirection;

    public bool isMoving;
    public bool isDodging;
    public bool isAttacking;
    public bool hadEffect;
    GameObject chestInRange;
    GameObject torchInRange;

    void Update()
    {
        Vector3 moveInput = Vector3.zero;
        if (Input.GetKey("w"))
            moveInput.z += 1;
        if (Input.GetKey("s"))
            moveInput.z -= 1;
        if ( Input.GetKey("d"))
            moveInput.x += 1;
        if (Input.GetKey("a"))
            moveInput.x -= 1;

        Vector3 attackInput = Vector3.zero;
        if (Input.GetKey("up"))
            attackInput.z += 1;
        if (Input.GetKey("down"))
            attackInput.z -= 1;
        if (Input.GetKey("right"))
            attackInput.x += 1;
        if (Input.GetKey("left"))
            attackInput.x -= 1;

        Move(moveInput);
        Dodge(moveInput);
        Attack(attackInput);
        Interact();
    }

    void Move(Vector3 inputDirection)
    {
        Vector3 movementDirection = inputDirection.normalized * speed;
        if (isAttacking)
            movementDirection *= attackMoveSpeedMod;
        else
            transform.LookAt(playerRb.position + movementDirection);

        playerRb.velocity = movementDirection;
        animator.SetFloat("Moving", movementDirection.magnitude);
        isMoving = true;
    }

    void Dodge(Vector3 inputDirection)
    {
        if (Input.GetKeyDown("space") && !isDodging && Time.time - dodgeEnd >= dodgeCooldown)
        {
            dodgeStart = Time.time;
            if (inputDirection.magnitude > 0)
                dodgeDirection = inputDirection.normalized * dodgeSpeed;
            else
                dodgeDirection = transform.forward * dodgeSpeed;

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

    void Attack(Vector3 inputDirection)
    {
        if (inputDirection.magnitude > 0 && !isDodging && !isAttacking && Time.time - attackStart >= attackCooldown)
        {
            attackStart = Time.time;
            animator.SetFloat("Attack_Melee", attackSpeed);
            isAttacking = true;
        }
        else if (isAttacking && Time.time - attackStart >= 1 / attackSpeed)
        {
            animator.SetFloat("Attack_Melee", 0);
            isAttacking = false;
            hadEffect = false;
        }
        else if (isAttacking && !hadEffect && Time.time - attackStart >= 1 / attackSpeed / 2)
        {
            playerCombat.Attack();
            hadEffect = true;
        }
        else if (isAttacking && Time.time - attackStart <= maxAttackDirectionInputTime)
            transform.LookAt(playerRb.position + inputDirection);

    }

    void Interact()
    {
        if(Input.GetKeyDown("e"))
        {
            if (chestInRange != null)
                chestInRange.GetComponent<ChestController>().Open();
            else if (torchInRange != null)
                torchInRange.GetComponent<TorchController>().Activate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Chest")
        {
            chestInRange = other.gameObject;
            UIController.Instance.SetInteractionTip("open");
        }
        else if (other.gameObject.tag == "Torch")
        {
            torchInRange = other.gameObject;
            UIController.Instance.SetInteractionTip("light");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == chestInRange)
        {
            chestInRange = null;
            UIController.Instance.RemoveInteractionTip();
        }
        else if (other.gameObject == torchInRange)
        {
            torchInRange = null;
            UIController.Instance.RemoveInteractionTip();
        }
    }
}
