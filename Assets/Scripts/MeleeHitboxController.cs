using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitboxController : MonoBehaviour
{
    public float duration;
    float damage;
    float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime >= duration)
            Destroy(gameObject);
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().DealDamage(damage, transform);
        }
        else if(other.CompareTag("Destroyable"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
