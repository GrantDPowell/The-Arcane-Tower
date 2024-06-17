using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int value; // Value of the XP or Gold
    public float pickupRange = 1.5f; // Range within which the item is picked up by the player
    public float explosionForce = 51f; // Force with which the items explode outwards
    public float rotationSpeed = 75f; // Speed at which the items rotate
    public float bobbingHeight = 0.25f; // Height of the bobbing effect
    public float bobbingSpeed = 1.5f; // Speed of the bobbing effect
    public float explosionDuration = 0.5f; // Duration for which the items will be affected by the explosion
    public float fixedYPosition = 0.5f; // Fixed y position for the drops

    private Transform playerTransform;
    private bool isAttracted = false;
    private Vector3 startPosition;
    private float bobbingOffset;
    private Rigidbody rb;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        if (playerTransform != null)
        {
            pickupRange = playerTransform.GetComponent<PlayerSystem>().playerStats.GetPickupRange();
        }

        // Add explosion force
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Generate a random explosion direction
            Vector3 explosionDirection = (Random.insideUnitSphere).normalized + Vector3.up;
            rb.AddForce(explosionDirection * explosionForce, ForceMode.Impulse);

            // Schedule to stop explosion effect after a duration
            Invoke(nameof(StopExplosion), explosionDuration);
        }

        // Set a fixed y position and initial rotation
        transform.position = new Vector3(transform.position.x, fixedYPosition, transform.position.z);
        transform.Rotate(15f, 0f, 0f); // Rotate by 25 degrees on the x-axis

        // Store the starting position for bobbing effect
        startPosition = transform.position;
        bobbingOffset = Random.Range(0f, 2 * Mathf.PI);
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            return; // Early exit if playerTransform is null
        }

        if (isAttracted)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            Vector3 direction = (playerTransform.position - transform.position).normalized;
            float speed = 5f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed);

            if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                Pickup();
            }
        }
        else
        {
            // Rotate the item
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Apply bobbing effect
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed + bobbingOffset) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Check for player proximity
            if (Vector3.Distance(transform.position, playerTransform.position) <= pickupRange)
            {
                isAttracted = true;
            }
        }
    }

    private void StopExplosion()
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            // Ensure the item stays at the fixed y position
            transform.position = new Vector3(transform.position.x, fixedYPosition, transform.position.z);
        }
    }

    private void Pickup()
    {
        PlayerStats playerStats = playerTransform.GetComponent<PlayerSystem>().playerStats;
        if (CompareTag("XP"))
        {
            playerTransform.GetComponent<PlayerSystem>().GainExperience(value);
        }
        else if (CompareTag("Gold"))
        {
            playerStats.coins += value;
        }
        else if (CompareTag("Gem"))
        {
            playerStats.gems += value;
        }
        Destroy(gameObject);
    }
}
