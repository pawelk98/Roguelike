using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody rb;
    public bool ownedByPlayer;
    public bool isPiercing;
    float damage;

    void Start()
    {
        this.transform.SetParent(GameObject.Find("Bullets").transform);
    }

    void Update()
    {
        
    }

    public void SetBullet(Vector3 direction, float damage, float lifetime)
    {
        this.damage = damage;
        rb.velocity = direction;

        StartCoroutine(HandleDestruction(lifetime));
    }

    IEnumerator HandleDestruction(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !ownedByPlayer)
        {
            if(other.gameObject.GetComponent<PlayerCombat>().DealRangedDamage(damage))
                Destroy(gameObject);
        }
        else if(other.CompareTag("Enemy") && ownedByPlayer)
        {
            other.gameObject.GetComponent<EnemyController>().DealDamage(damage);
            if (!isPiercing)
                Destroy(gameObject);
        }
        else if(other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}
