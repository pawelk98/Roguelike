using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float maxAttackDirectionInputTime = 0.2f;

    public Rigidbody playerRb;
    public Animator animator;
    public PlayerCombat playerCombat;

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
        if(!UIController.Instance.IsShopOpened())
        {
            if (Input.GetKey("up"))
                attackInput.z += 1;
            if (Input.GetKey("down"))
                attackInput.z -= 1;
            if (Input.GetKey("right"))
                attackInput.x += 1;
            if (Input.GetKey("left"))
                attackInput.x -= 1;
        }

        Move(moveInput);
        Dodge(moveInput);
        Attack(attackInput);
        Interact();
    }

    void Move(Vector3 inputDirection)
    {
        Vector3 movementDirection = inputDirection.normalized * speed;
        if (isAttacking)
            movementDirection *= PlayerInventory.Instance.currentWeapon.attackMoveSpeedMod;
        else
            transform.LookAt(playerRb.position + movementDirection);

        playerRb.velocity = movementDirection;
        animator.SetFloat("Moving", movementDirection.magnitude);
        isMoving = true;
    }

    void Dodge(Vector3 inputDirection)
    {
        if (Input.GetKeyDown("space") && !isDodging && Time.time - dodgeEnd >= PlayerInventory.Instance.currentWeapon.dodgeCooldown)
        {
            dodgeStart = Time.time;
            if (inputDirection.magnitude > 0)
                dodgeDirection = inputDirection.normalized * PlayerInventory.Instance.currentWeapon.dodgeSpeed;
            else
                dodgeDirection = transform.forward * PlayerInventory.Instance.currentWeapon.dodgeSpeed;

            animator.SetFloat("Dodging", 1 / PlayerInventory.Instance.currentWeapon.dodgeDuration);
            isDodging = true;
        }

        if (isDodging)
        {
            playerRb.velocity = dodgeDirection;
            transform.LookAt(playerRb.position + dodgeDirection);

            if (Time.time >= dodgeStart + PlayerInventory.Instance.currentWeapon.dodgeDuration)
            {
                dodgeEnd = Time.time;
                isDodging = false;
                animator.SetFloat("Dodging", 0);
            }
        }
    }

    void Attack(Vector3 inputDirection)
    {
        string animationTag;

        switch(PlayerInventory.Instance.currentWeapon.type)
        {
            case 0:
                animationTag = "Attack_1H";
                break;
            case 1:
                animationTag = "Attack_1H";
                break;
            case 2:
                animationTag = "Attack_2H";
                break;
            case 3:
                animationTag = "Attack_Bow";
                break;
            default:
                animationTag = "Attack_1H";
                break;
        }

        if (inputDirection.magnitude > 0 && !isDodging && !isAttacking && Time.time - attackStart >= PlayerInventory.Instance.currentWeapon.attackCooldown)
        {
            attackStart = Time.time;
            animator.SetFloat(animationTag, PlayerInventory.Instance.currentWeapon.attackSpeed);
            isAttacking = true;
        }
        else if (isAttacking && Time.time - attackStart >= 1 / PlayerInventory.Instance.currentWeapon.attackSpeed)
        {
            animator.SetFloat(animationTag, 0);
            isAttacking = false;
            hadEffect = false;
        }
        else if (isAttacking && !hadEffect && Time.time - attackStart >= 1 / PlayerInventory.Instance.currentWeapon.attackSpeed / 2)
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
        if(other.gameObject.CompareTag("Chest"))
        {
            chestInRange = other.gameObject;
            UIController.Instance.SetInteractionTip("open");
        }
        else if (other.gameObject.CompareTag("Torch"))
        {
            torchInRange = other.gameObject;
            UIController.Instance.SetInteractionTip("light");
        }
        else if (other.gameObject.CompareTag("WeaponShop"))
            UIController.Instance.OpenWeaponShop();
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
        else if (other.gameObject.CompareTag("WeaponShop"))
            UIController.Instance.CloseWeaponShop();
    }
}
