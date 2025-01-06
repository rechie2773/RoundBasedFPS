using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoundManager : MonoBehaviour
{
    public int initialZombiesPerRound = 5;
    public int currentZombiesPerRound;

    public float spawnDelay = 1.5f;

    public int currentRound = 0;
    public float RoundCoolDown = 20.0f;
    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<EnemyAI> currentZombiesAlive;

    public GameObject zombiePref; //zombie prefab/model
    public List<Transform> spawnPoints;
    public AudioSource roundChangeAudio;

    public float zombieSpeed = 3.5f; //base speed
    public float speedIncreasePerRound = 0.2f; // increase speed unit every round
    public float maxZombieSpeed = 10f; // limit speed

    public float baseZombieHealth = 100f; //base health
    public float healthIncreasePerRound = 20f; // increase hp per round
    public float maxZombieHealth = 700f; // health limit

    private RoundUI roundUI;

    private void Start()
    {
        currentZombiesPerRound = initialZombiesPerRound;
        StartNextRound();
        RoundUI roundUI = FindObjectOfType<RoundUI>();
        if (roundUI != null)
        {
            roundUI.TriggerRoundTransition(currentRound);
        }
    }
    private void StartNextRound()
    {
        currentZombiesAlive.Clear();

        currentRound++;
        //round transition
        RoundUI roundUI = FindObjectOfType<RoundUI>();
        if (roundUI != null)
        {
            roundUI.TriggerRoundTransition(currentRound);
        }
        //increase zombie's speed per round
        zombieSpeed = Mathf.Min(zombieSpeed + speedIncreasePerRound, maxZombieSpeed);

        //increase zombie's hp per round
        baseZombieHealth = Mathf.Min(baseZombieHealth + healthIncreasePerRound, maxZombieHealth);
        StartCoroutine(spawnRound());
    }
    private IEnumerator spawnRound()
    {
        for (int i = 0; i < currentZombiesPerRound; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var zombie = Instantiate(zombiePref, spawnPoint.position, spawnPoint.rotation);

            NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = zombieSpeed;
            }
            EnemyAI enemyScript = zombie.GetComponent<EnemyAI>();

            if (enemyScript != null)
            {
                enemyScript.health = baseZombieHealth; //update zombie's hp
                currentZombiesAlive.Add(enemyScript);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    private void Update()
    {
        //get all dead zombies
        List<EnemyAI> zombiesToRemove =  new List<EnemyAI>();
        foreach (EnemyAI zombie in currentZombiesAlive)
        {
            if (zombie.IsDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }
        foreach (EnemyAI zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }
        zombiesToRemove.Clear();
        //start countdown if all zombies are dead
        if(currentZombiesAlive.Count == 0 && inCooldown == false)
        {
            //start cooldown for next wave
            StartCoroutine(StartRoundCoolDown());
        }

        //run cooldown counter
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = RoundCoolDown;
        }
    }
    private IEnumerator StartRoundCoolDown()
    {
        inCooldown = true;

        // Play round change audio
        if (roundChangeAudio != null)
        {
            roundChangeAudio.Play();
        }

        float cooldownTime = RoundCoolDown; // Total cooldown time

        while (cooldownTime > 0)
        {
            if (PauseMenu.paused)
            {
                if (roundChangeAudio != null && roundChangeAudio.isPlaying)
                {
                    roundChangeAudio.Pause();
                }

                yield return null;
                continue;
            }
            else
            {
                if (roundChangeAudio != null && !roundChangeAudio.isPlaying)
                {
                    roundChangeAudio.UnPause();
                }
            }
            if (cooldownTime == 10.0f) // At 10 seconds, increase the round
            {
                currentRound++;
                Debug.Log($"Round incremented early to: {currentRound}");        
            }

            cooldownTime -= Time.deltaTime; // Decrease cooldown time
            yield return null; // Wait for the next frame
        }

        inCooldown = false;
        currentZombiesPerRound +=5; // +5 zombies every advanced round
        StartNextRound();
    }
}
