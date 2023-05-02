using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float health;
    public float currentHealth;

    public float damage;

    public PlayerMovementScript playerMovementScript;
    public GameObject meleeColliderPrefab;
    public Transform attackOrigin;
    public Material flashMaterial;
    public float damageFlashDuration;
    public List<SkinnedMeshRenderer> renderers;
    Material baseMaterial;
    float damageFlashStart;

    void Start()
    {
        baseMaterial = renderers[0].material;
        UIController.Instance.SetHealth((int)currentHealth);
    }

    void Update()
    {
        FlashHandler();
    }

    public bool DealMeleeDamage(float damage)
    {
        if (!playerMovementScript.isDodging)
        {
            currentHealth -= damage;
            FlashHandler(true);
            UIController.Instance.SetHealth((int)currentHealth);
            isAlive();
            return true;
        }
        return false;
    }

    public bool DealRangedDamage(float damage)
    {
        if(!playerMovementScript.isDodging)
        {
            currentHealth -= damage;
            FlashHandler(true);
            UIController.Instance.SetHealth((int)currentHealth);
            isAlive();
            return true;
        }
        return false;
    }

    void isAlive()
    {
        if (currentHealth <= 0)
            UIController.Instance.SetHealth(0);
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
        GameObject meleeCollider = Instantiate(meleeColliderPrefab, attackOrigin.transform.position, Quaternion.LookRotation(transform.rotation.eulerAngles, Vector3.up));
        meleeCollider.GetComponent<MeleeHitboxController>().SetDamage(damage);
    }

    void FlashHandler(bool tookDamage = false)
    {
        if (tookDamage)
        {
            damageFlashStart = Time.time;
            foreach (SkinnedMeshRenderer s in renderers)
                s.material = flashMaterial;
        }
        else if (Time.time - damageFlashStart >= damageFlashDuration)
            foreach (SkinnedMeshRenderer s in renderers)
                s.material = baseMaterial;
    }
}
