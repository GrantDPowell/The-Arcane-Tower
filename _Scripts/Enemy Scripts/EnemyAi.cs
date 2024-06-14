using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;
    private Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public EnemyStats enemyStats;
    private float health;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    private float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private bool seenPlayer = false;
    private PlayerSystem playerComponent;
    private int playerLevel;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false; // Disable the agent initially

        BaseLevelGenerator.OnNavMeshReady += EnableAgent; // Subscribe to the NavMesh ready event

        health = enemyStats.GetHealth();
    }

    private void OnDestroy()
    {
        BaseLevelGenerator.OnNavMeshReady -= EnableAgent; // Unsubscribe to avoid memory leaks
    }

    private void EnableAgent()
    {
        RepositionOnNavMesh();
        agent.enabled = true; // Enable the agent after repositioning
        agent.speed = enemyStats.GetMoveSpeed();
    }

    private void Start()
    {
        // Scale enemy stats based on player level
        playerComponent = FindObjectOfType<PlayerSystem>();
        if (playerComponent != null)
        {
            int playerLevel = playerComponent.playerStats.level;
            enemyStats.ScaleStats(playerLevel);
            health = Mathf.Min(health, enemyStats.GetHealth()); // Ensure health does not exceed max health after scaling
        }
        timeBetweenAttacks = 3 - enemyStats.GetAttackCooldown();
    }

    private void Update()

    {
        if (playerLevel != playerComponent.playerStats.level)
        {
            playerLevel = playerComponent.playerStats.level;
            enemyStats.ScaleStats(playerLevel);
            health = Mathf.Min(health, enemyStats.GetHealth());
        }
        if (!agent.enabled) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player"); // Use tag to find player
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                return;
            }
        }

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange || seenPlayer)
        {
            ChasePlayer();
            seenPlayer = true;
        }
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position), 5f * Time.deltaTime);

        if (!alreadyAttacked)
        {
            // Instantiate and shoot the magic missile
            ShootMagicMissile();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ShootMagicMissile()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;

        GameObject magicMissile = Instantiate(projectile, transform.position + transform.forward, Quaternion.LookRotation(direction));
        EnemyMissile missileComponent = magicMissile.GetComponent<EnemyMissile>();
        if (missileComponent != null)
        {
            float damage = enemyStats.GetDamage();
            float speed = enemyStats.GetSpellSpeed(); // Use enemy's wisdom for spell speed
            float range = enemyStats.GetSpellRange(); // Use enemy's wisdom for spell range
            missileComponent.Initialize((int)damage, speed, range, projectile);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        if (player != null)
        {
            PlayerSystem playerComponent = player.GetComponent<PlayerSystem>();
            if (playerComponent != null)
            {
                playerComponent.playerStats.totalDamageDealt += damage;
            }
        }
        float defense = enemyStats.GetDefense();
        float actualDamage = damage - defense;
        if (actualDamage < 0) actualDamage = 0;

        health -= actualDamage;

       // Debug.Log("Enemy took Damage");

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.1f);
    }

    private void DestroyEnemy()
    {
        NotifyPlayerOfDeath();
        DropItems();    
        Destroy(gameObject);
    }
    private void DropItems()
    {
        DropManager dropManager = FindObjectOfType<DropManager>();
        if (dropManager != null)
        {
            dropManager.DropItems(transform.position, enemyStats.currentExperiencePoints, enemyStats.currentGoldDrop);
        }
        else
        {
            Debug.LogError("DropManager not found in the scene.");
        }
    }

    private void NotifyPlayerOfDeath()
    {
        //Debug.Log("Enemy died");

        if (player != null)
        {
            PlayerSystem playerComponent = player.GetComponent<PlayerSystem>();
            if (playerComponent != null)
            {
                //playerComponent.GainExperience(enemyStats.GetExperiencePoints());
                //Debug.Log("Player gained " + enemyStats.GetExperiencePoints() + " experience points.");

                playerComponent.playerStats.totalMonstersKilled++;

            }
            else
            {
                //Debug.Log("Player component not found to notify of enemy death.");
            }
        }
        else
        {
            //Debug.Log("Player not found to notify of enemy death.");
        }
    }

    private void RepositionOnNavMesh()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
