using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public int damage;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Attack control
    private float timeBetweenAttacks = 1.5f; // Time interval between attacks
    private bool alreadyAttacked;
    public AudioSource hitmarker;
    private Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] chaseSounds;   
    public AudioClip[] attackSounds; 
    public float audioCooldown = 2f; 
    private float nextAudioTime;

    [Header("Power-Up Settings")]
    public GameObject[] powerUpPrefabs; 
    [Range(0f, 1f)]
    public float powerUpSpawnChance = 0.2f; //chance to spawn

    private bool isDead = false; //check is enemy is dead

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        if (playerInAttackRange && playerInSightRange)
            AttackPlayer();

        // Update animator parameters
        animator.SetBool("IsAttacking", playerInAttackRange); // Toggle attack animation
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        audioSource.volume = Mathf.Clamp((1f - (distanceToPlayer / 50f)) * 0.25f, 0f, 0.25f); // Adjust volume based on distance
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        if (Time.time >= nextAudioTime && chaseSounds.Length > 0)
        {
            PlayRandomSound(chaseSounds);
            nextAudioTime = Time.time + audioCooldown;
        }
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Apply damage to the player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage); // Deal damage to the player
            }

            if (attackSounds.Length > 0)
            {
                PlayRandomSound(attackSounds);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    private void PlayRandomSound(AudioClip[] sounds)
    {
        if (audioSource != null)
        {
            AudioClip clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.PlayOneShot(clip, 0.25f);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        health -= damage;

        //display hitmarker
        HitMarker hitmarkerManager = FindObjectOfType<HitMarker>();
        if (hitmarkerManager != null)
        {
            hitmarkerManager.ShowHitmarker();
        }

        // add 10 points for dealing damage
        if (PointSystem.Instance != null)
        {
            PointSystem.Instance.AddPoints(10);
        }
        if (hitmarker != null)
        {
            hitmarker.Play();
        }
        if (health <= 0)
        {
            if (!isDead)
            {
                isDead = true;

                Debug.Log($"Enemy {gameObject.name} killed.");

                ScoreboardManager scoreboardManager = FindObjectOfType<ScoreboardManager>();
                if (scoreboardManager != null)
                {
                    scoreboardManager.ZombieKilled();
                }
                if (PointSystem.Instance != null)
                {
                    PointSystem.Instance.AddPoints(100);
                    Debug.Log($"Added 100 points for killing {gameObject.name}");
                }
                TrySpawnPowerUp();
                Destroy(gameObject);
            }
        }

    }
        

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    public bool IsDead
    {
        get { return health <= 0; }
    }
    private void TrySpawnPowerUp()
    {
        if (powerUpPrefabs.Length == 0) return;

        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= powerUpSpawnChance)
        {
            //random power up
            GameObject selectedPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

            //spawn power up at death's location
            Instantiate(selectedPowerUp, transform.position, Quaternion.identity);
            Debug.Log("Power-up spawned!");
        }
    }
}
