using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public GameObject explosionPrefab; // Assign the explosion prefab here
    private int damage; // Damage dealt by the magic missile
    private float speed; // Speed of the magic missile
    private float range; // Range of the magic missile
    private GameObject splitPrefab;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from EnemyMissile prefab.");
        }
    }

    public void Initialize(int damage, float speed, float range, GameObject splitPrefab)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.splitPrefab = splitPrefab;

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody component is missing, cannot set velocity.");
        }

        Destroy(gameObject, range / speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Spell"))
        {
            // phase through other spells
            return;
        }

        Destroy(gameObject);
    }


}
