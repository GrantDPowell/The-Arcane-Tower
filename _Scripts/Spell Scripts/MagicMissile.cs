using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    public GameObject explosionPrefab; // Assign the explosion prefab here
    private int damage; // Damage dealt by the magic missile
    private float speed; // Speed of the magic missile
    private float range; // Range of the magic missile
    private bool isHoming;
    private int splitCount;
    private GameObject splitPrefab;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from MagicMissile prefab.");
        }
    }

    public void Initialize(int damage, float speed, float range, bool isHoming, int splitCount, GameObject splitPrefab)
    {
        this.damage = damage;
        this.speed = speed;
        this.range = range;
        this.isHoming = isHoming;
        this.splitCount = splitCount;
        this.splitPrefab = splitPrefab;

        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Rigidbody component is missing, cannot set velocity.");
        }

        if (isHoming)
        {
            InvokeRepeating("FindClosestEnemy", 0.5f, 0.5f);
        }
        

        Destroy(gameObject, range / speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAi enemy = collision.gameObject.GetComponent<EnemyAi>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Spell"))
        {
            // phase through other spells
            return;
        }
        if (splitCount > 0)
        {
            Split();
        }

        

        Destroy(gameObject);
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            Vector3 directionToEnemy = (closestEnemy.transform.position - currentPosition).normalized;
            rb.velocity = directionToEnemy * speed;
        }
    }

    private void Split()
    {
        for (int i = 0; i < splitCount; i++)
        {
            Debug.Log("Splitting missile");
            float angle = i * (360 / splitCount);
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 direction = rotation * transform.forward;
            // make the shop appear higher in the y axis
            Transform upperPosition = transform;
            upperPosition.position = new Vector3(upperPosition.position.x, upperPosition.position.y + 1, upperPosition.position.z);

            GameObject splitMissile = Instantiate(splitPrefab, upperPosition.position, rotation);
            splitMissile.GetComponent<MagicMissile>().Initialize(damage, speed, range / 2, isHoming, i, null);
        }
    }
}
