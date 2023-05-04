using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int quantity;
    public float health;
    float currentHealth;
    public float meleeAttackRange;
    public float rangedAttackRange;
    public float meleeAttackDamage;
    public float rangedAttackDamage;
    public float attackSpeed;
    public float bulletSpeed;
    public float mass;
    public bool isRanged;
    public bool isMelee;
    public float damageFlashDuration;
    public float deadBodyDuration;
    float damageFlashStart;
    float lastAttack;

    public float alertRange;
    public float scanRate = 1;
    float lastScan;

    bool isAttacking;
    bool hadEffect;
    bool isAlerted;

    Vector3 towardsPlayer;

    public NavMeshAgent agent;
    public Animator animator;
    public GameObject bulletPrefab;
    public Transform bulletOrigin;
    public Rigidbody rigidBody;
    public LayerMask layerMask;
    public Collider collid;
    Material baseMaterial;
    public Material flashMaterial;
    public List<SkinnedMeshRenderer> renderers;
    GameObject player;
    PlayerCombat playerCombat;
    bool isAlive = true;

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
            return;
        }

        if (player != null)
        {
            towardsPlayer = player.transform.position - transform.position;
            if (!isAlerted)
                ScanAlert();
            else
            {
                RaycastHit hit;

                float raycastRange;
                if (isRanged)
                    raycastRange = rangedAttackRange;
                else
                    raycastRange = meleeAttackRange;

                Physics.Raycast(transform.position, towardsPlayer, out hit, raycastRange, layerMask);

                if (isAttacking || hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    Attack();
                }
                else if (!isAttacking)
                {
                    Move();
                }

            }
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                playerCombat = player.GetComponent<PlayerCombat>();
        }

        CheckAttackEnd();
        FlashHandler();
        animator.SetFloat("Running", agent.velocity.magnitude);
    }

    void ScanAlert()
    {
        if(Time.time - lastScan >= 1/scanRate)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, towardsPlayer, out hit, alertRange, layerMask);

            if (hit.collider && hit.collider.CompareTag("Player"))
            {
                isAlerted = true;
            }
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
        transform.LookAt(player.transform);

        float distanceFromPlayer = towardsPlayer.magnitude;

        if (isMelee && distanceFromPlayer <= meleeAttackRange)
            AttackMelee();
        else if (isRanged && distanceFromPlayer <= rangedAttackRange)
            AttackRanged();
    }

    void AttackMelee()
    {
        if(isAlerted && !isAttacking && towardsPlayer.magnitude <= meleeAttackRange)
        {
            animator.SetFloat("Attack_Melee", attackSpeed);
            isAttacking = true;
            lastAttack = Time.time;
        }
        else if (isAttacking && !hadEffect && Time.time - lastAttack >= 1 / attackSpeed / 2)
        {
            if (towardsPlayer.magnitude <= meleeAttackRange)
                playerCombat.DealMeleeDamage(meleeAttackDamage);
            hadEffect = true;
        }
    }

    void CheckAttackEnd()
    {
        if (isAttacking && Time.time - lastAttack >= 1 / attackSpeed)
        {
            animator.SetFloat("Attack_Melee", 0);
            animator.SetFloat("Attack_Ranged", 0);
            isAttacking = false;
            hadEffect = false;
        }
    }

    void AttackRanged()
    {
        if (isAlerted && !isAttacking && towardsPlayer.magnitude <= rangedAttackRange)
        {
            animator.SetFloat("Attack_Ranged", attackSpeed);
            isAttacking = true;
            lastAttack = Time.time;
        }
        else if (isAttacking && !hadEffect && Time.time - lastAttack >= 1 / attackSpeed / 2)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.LookRotation(towardsPlayer, Vector3.up));
            bullet.GetComponent<BulletController>().setBullet(towardsPlayer.normalized * bulletSpeed, rangedAttackDamage);
            hadEffect = true;
        }
    }

    public void DealDamage(float damage, Transform source = null)
    {
        if (!isAlive)
            return;

        currentHealth -= damage;
        StartCoroutine(FlashHandler(true));

        StartCoroutine(IsAlive());

        if(source != null)
            agent.velocity = (transform.position - source.position).normalized * damage / mass;
    }

    IEnumerator IsAlive()
    {
        if (currentHealth <= 0)
        {
            UIController.Instance.RemoveEnemy();
            RoomChange.currentRoom.GetComponent<RoomController>().KillEnemy(gameObject);
            animator.SetBool("IsDead", true);
            isAlive = false;

            yield return new WaitForSeconds(deadBodyDuration);
            Destroy(gameObject);
        }
    }

    public void Alert()
    {
        isAlerted = true;
    }

    IEnumerator FlashHandler(bool tookDamage = false)
    {
        if(tookDamage)
        {
            damageFlashStart = Time.time;
            foreach (SkinnedMeshRenderer s in renderers)
                s.material = flashMaterial;

            yield return new WaitForSeconds(damageFlashDuration);
            foreach (SkinnedMeshRenderer s in renderers)
                s.material = baseMaterial;
        }
    }
}
