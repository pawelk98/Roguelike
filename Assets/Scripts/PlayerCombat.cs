using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombat : MonoBehaviour
{
    public float health;
    public float damageFlashDuration;
    public float damage;
    public float armor;

    public PlayerController playerController;
    public GameObject[] meleeColliderPrefabs;
    public GameObject[] bullets;
    public Transform meleeAttackOrigin;
    public Transform rangedAttackOrigin;
    public Material flashMaterial;
    public List<SkinnedMeshRenderer> renderers;

    Material baseMaterial;
    float currentHealth;

    void Start()
    {
        currentHealth = health;
        baseMaterial = renderers[0].material;
        UIController.Instance.SetHealth((int)currentHealth);
    }

    void Update()
    {
        FlashHandler();
    }

    public bool DealMeleeDamage(float damage)
    {
        if (!playerController.isDodging)
        {
            currentHealth -= damage / armor;
            StartCoroutine(FlashHandler());
            UIController.Instance.SetHealth((int)currentHealth);
            isAlive();
            return true;
        }
        return false;
    }

    public bool DealRangedDamage(float damage)
    {
        if(!playerController.isDodging)
        {
            currentHealth -= damage / armor;
            StartCoroutine(FlashHandler());
            UIController.Instance.SetHealth((int)currentHealth);
            isAlive();
            return true;
        }
        return false;
    }

    void isAlive()
    {
        if (currentHealth <= 0)
            Save.Instance.SaveAndRestart();
    }

    public void Heal(float ammount)
    {
        currentHealth += ammount;

        if (currentHealth > health)
            currentHealth = health;

        UIController.Instance.SetHealth((int)currentHealth);
    }

    public void Attack()
    {
        GameObject attackObject = null;

        switch (PlayerInventory.Instance.currentWeapon.type)
        {
            case 0:
                attackObject = meleeColliderPrefabs[0];
                break;
            case 1:
                attackObject = meleeColliderPrefabs[1];
                break;
            case 2:
                attackObject = meleeColliderPrefabs[2];
                break;
            case 3:
                attackObject = bullets[0];
                break;
            case 4:
                attackObject = bullets[1];
                break;
            case 5:
                attackObject = bullets[2];
                break;
        }

        if (PlayerInventory.Instance.currentWeapon.type <= 2)    //melee
        {
            GameObject meleeCollider = Instantiate(attackObject, meleeAttackOrigin.transform);
            meleeCollider.GetComponent<MeleeHitboxController>().SetDamage(damage);
        }
        else   //ranged
        {
            Vector3 direction;

            switch(PlayerInventory.Instance.currentWeapon.type)
            {
                case 3:
                    Shoot(attackObject, transform.forward);
                    break;
                case 4:
                    direction = Quaternion.AngleAxis(10f, Vector3.up) * transform.forward;
                    Shoot(attackObject, direction);
                    direction = transform.forward;
                    Shoot(attackObject, direction);
                    direction = Quaternion.AngleAxis(-10f, Vector3.up) * transform.forward;
                    Shoot(attackObject, direction);
                    break;
                case 5:
                    direction = Quaternion.AngleAxis(Random.Range(-2f, 2f), Vector3.up) * transform.forward;
                    Shoot(attackObject, direction);
                    break;
            }
        }
    }

    void Shoot(GameObject bulletPrefab, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, rangedAttackOrigin);
        bullet.GetComponent<BulletController>().SetBullet(
            direction * PlayerInventory.Instance.currentWeapon.bulletSpeed,
            damage, PlayerInventory.Instance.currentWeapon.bulletLifetime);
    }

    IEnumerator FlashHandler()
    {
        foreach (SkinnedMeshRenderer s in renderers)
            s.material = flashMaterial;

        yield return new WaitForSeconds(damageFlashDuration);

        foreach (SkinnedMeshRenderer s in renderers)
            s.material = baseMaterial;
    }
}
