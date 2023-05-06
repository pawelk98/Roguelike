using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int quantity;
    public int reward;
    public float mass;
    public float health;
    public float attackSpeed;
    [Range(0,1)]
    public float attackDamageDelay;
    public bool isRanged;
    public float rangedAttackDamage;
    public float rangedAttackRange;
    public float bulletSpeed;
    public float bulletLifetime = 10f;
    public bool isMelee;
    public float meleeAttackDamage;
    public float meleeAttackRange;
    public float damageFlashDuration;
    public float corpseDuration;
    public float alertRange;
    public float scanRate = 1;
    public float currentHealth;

    bool isAttacking;
    bool isAlive = true;
    bool isAlerted;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject bulletPrefab;
    public Transform bulletOrigin;
    public Rigidbody rigidBody;
    public LayerMask layerMask;
    public Collider collid;
    public List<SkinnedMeshRenderer> renderers;
    public Material flashMaterial;

    Material baseMaterial;
    GameObject player;
    PlayerCombat playerCombat;
    IEnumerator scanCoroutine;

    Vector3 towardsPlayer;

    void Start()
    {
        baseMaterial = renderers[0].material;
        currentHealth = health;

        player = GameObject.Find("Player");
        if (player)
            playerCombat = player.GetComponent<PlayerCombat>();

        UIController.Instance.AddEnemy();
    }

    void Update()
    {
        if (!isAlive)
        {
            agent.enabled = false;
            collid.enabled = false;
            rigidBody.velocity = Vector3.zero;
            return;
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                playerCombat = player.GetComponent<PlayerCombat>();
            else
                return;
        }

        towardsPlayer = player.transform.position - transform.position;

        if (scanCoroutine == null && !isAlerted)
        {
            scanCoroutine = ScanAlert();
            StartCoroutine(scanCoroutine);
        }

        if (!isAlerted)
            return;

        if (isAttacking)
        {
            transform.LookAt(player.transform);
            return;
        }

        RaycastHit hit;
        float raycastRange;
        if (isRanged)
            raycastRange = rangedAttackRange;
        else
            raycastRange = meleeAttackRange;

        Physics.Raycast(transform.position, towardsPlayer, out hit, raycastRange, layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            animator.SetFloat("Running", 0);
            Attack();
        }
        else
        {
            if(isMelee)
                animator.SetFloat("Attack_Melee", 0);
            if(isRanged)
                animator.SetFloat("Attack_Ranged", 0);

            animator.SetFloat("Running", agent.velocity.magnitude);
            Move();
        }
    }

    IEnumerator ScanAlert()
    {
        while(true)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, towardsPlayer, out hit, alertRange, layerMask);

            if (hit.collider && hit.collider.CompareTag("Player"))
            {
                isAlerted = true;
                StopCoroutine(scanCoroutine);
            }

            yield return new WaitForSeconds(1/scanRate);
        }
    }

    void Move()
    {
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        transform.LookAt(transform.position + agent.velocity);
    }

    void Attack()
    {
        agent.isStopped = true;
        float distanceFromPlayer = towardsPlayer.magnitude;

        if (isRanged && distanceFromPlayer >= meleeAttackRange)
            StartCoroutine(AttackRanged());
        else if (isMelee)
            StartCoroutine(AttackMelee());
    }

    IEnumerator AttackMelee()
    {
        isAttacking = true;
        animator.SetFloat("Attack_Melee", attackSpeed);

        yield return new WaitForSeconds((1 / attackSpeed) * attackDamageDelay);

        if (towardsPlayer.magnitude <= meleeAttackRange && isAlive)
            playerCombat.DealMeleeDamage(meleeAttackDamage);

        yield return new WaitForSeconds((1 / attackSpeed) - (1 / attackSpeed) * attackDamageDelay);
        isAttacking = false;
    }

    IEnumerator AttackRanged()
    {
        isAttacking = true;
        animator.SetFloat("Attack_Ranged", attackSpeed);

        yield return new WaitForSeconds(1 / attackSpeed * attackDamageDelay);

        if(isAlive)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.LookRotation(towardsPlayer, Vector3.up));
            bullet.GetComponent<BulletController>().SetBullet(towardsPlayer.normalized * bulletSpeed, rangedAttackDamage, bulletLifetime);
        }

        yield return new WaitForSeconds((1 / attackSpeed) - (1 / attackSpeed) * attackDamageDelay);
        isAttacking = false;
    }

    IEnumerator IsAlive()
    {
        if (currentHealth <= 0)
        {
            UIController.Instance.RemoveEnemy();
            RoomChange.currentRoom.GetComponent<RoomController>().KillEnemy(gameObject);
            animator.SetBool("IsDead", true);
            isAlive = false;
            PlayerInventory.Instance.AddCoins(reward);

            yield return new WaitForSeconds(corpseDuration);
            Destroy(gameObject);
        }
    }

    IEnumerator DamageFlash()
    {
        foreach (SkinnedMeshRenderer s in renderers)
            s.material = flashMaterial;
        yield return new WaitForSeconds(damageFlashDuration);
        foreach (SkinnedMeshRenderer s in renderers)
            s.material = baseMaterial;
    }

    public void DealDamage(float damage, Transform source = null)
    {
        if (!isAlive)
            return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());
        StartCoroutine(IsAlive());

        if(source != null)
            agent.velocity = (transform.position - source.position).normalized * damage / mass;
    }

    public void Alert()
    {
        isAlerted = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("Player"))
            rigidBody.velocity = Vector3.zero;
    }
}
