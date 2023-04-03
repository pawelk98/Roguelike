using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody rb;
    float damage;

    void Start()
    {
        this.transform.SetParent(GameObject.Find("Bullets").transform);
    }

    void Update()
    {
        
    }

    public void setBullet(Vector3 direction, float damage)
    {
        this.damage = damage;
        rb.velocity = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<PlayerCombat>().DealRangedDamage(damage))
                Destroy(gameObject);
        }
        else if(other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
