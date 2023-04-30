using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float health;
    public float currentHealth;
    public float attackRange;
    public float attackDamage;
    public float attackSpeed;
    public float bulletSpeed;
    public float mass;
    public bool isRanged;
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
    GameObject player;
    PlayerCombat playerCombat;

    void Start()
    {
        player = GameObject.Find("Player");
        if (player)
            playerCombat = player.GetComponent<PlayerCombat>();

        UIController.Instance.AddEnemy();
    }

    void Update()
    {
        if(player != null)
        {
            towardsPlayer = player.transform.position - transform.position;
            if (!isAlerted)
                ScanAlert();
            else
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, towardsPlayer, out hit, attackRange, layerMask);

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

        if (!isRanged)
            AttackMelee();
        else
            AttackRanged();
    }

    void AttackMelee()
    {
        if(isAlerted && !isAttacking && towardsPlayer.magnitude <= attackRange)
        {
            animator.SetFloat("Attack_Melee", attackSpeed);
            isAttacking = true;
            lastAttack = Time.time;
        }
        else if (isAttacking && Time.time - lastAttack >= 1 / attackSpeed)
        {
            animator.SetFloat("Attack_Melee", 0);
            isAttacking = false;
            hadEffect = false;
        }
        else if (isAttacking && !hadEffect && Time.time - lastAttack >= 1 / attackSpeed / 2)
        {
            if (towardsPlayer.magnitude <= attackRange)
                playerCombat.DealMeleeDamage(attackDamage);
            hadEffect = true;
        }
    }

    void AttackRanged()
    {
        if (isAlerted && !isAttacking && towardsPlayer.magnitude <= attackRange)
        {
            animator.SetFloat("Attack_Melee", attackSpeed);
            isAttacking = true;
            lastAttack = Time.time;
        }
        else if (isAttacking && Time.time - lastAttack >= 1 / attackSpeed)
        {
            animator.SetFloat("Attack_Melee", 0);
            isAttacking = false;
            hadEffect = false;
        }
        else if (isAttacking && !hadEffect && Time.time - lastAttack >= 1 / attackSpeed / 2)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.LookRotation(towardsPlayer, Vector3.up));
            bullet.GetComponent<BulletController>().setBullet(towardsPlayer.normalized * bulletSpeed, attackDamage);
            hadEffect = true;
        }
    }

    public void DealDamage(float damage)
    {
        currentHealth -= damage;
        IsAlive();
    }

    void IsAlive()
    {
        if (currentHealth <= 0)
        {
            UIController.Instance.RemoveEnemy();
            RoomChange.currentRoom.GetComponent<RoomController>().KillEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    public void Alert()
    {
        isAlerted = true;
    }

}
