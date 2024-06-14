using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int value; // Value of the XP or Gold
    public float pickupRange = 1.5f; // Range within which the item is picked up by the player

    private Transform playerTransform;
    private bool isAttracted = false;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        pickupRange = playerTransform.GetComponent<PlayerSystem>().playerStats.GetPickupRange();
    }

    private void Update()
    {
        if (isAttracted)
        {
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
            if (Vector3.Distance(transform.position, playerTransform.position) <= pickupRange)
            {
                isAttracted = true;
            }
        }
    }

    private void Pickup()
    {
        PlayerStats playerStats = playerTransform.GetComponent<PlayerSystem>().playerStats;
        if (CompareTag("XP"))
        {
            ///playerStats.experiencePoints += value;
            playerTransform.GetComponent<PlayerSystem>().GainExperience(value);
        }
        else if (CompareTag("Gold"))
        {
            playerStats.coins += value;
        }
        Destroy(gameObject);
    }
}
