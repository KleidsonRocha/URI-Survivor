using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeTopSpawn;
    private float spawnCounter;

    public Transform minSpawn, maxSpawn;

    private Transform target;

    private float despawnDistance;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int checkPerFrame;
    private int enemyToCheck;

    public List<WaveInfo> waves;

    private int currentWave;

    private float waveCounter;


    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        if (Random.Range(0f, 1f) > .5f)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            }
            else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        } else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);

            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
        }
            return spawnPoint;
    }

    void Start()
    {
        //spawnCounter = timeTopSpawn;

        target = FindAnyObjectByType<PlayerController>().transform;

        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;

        currentWave = -1;
        GoToNextWave();
    }

    void Update()
    {
        /*
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0f)
        {
            spawnCounter = timeTopSpawn;

            //Instantiate(enemyToSpawn, transform.position, transform.rotation);
           GameObject newEnemy =  Instantiate(enemyToSpawn, SelectSpawnPoint() , transform.rotation);

           spawnedEnemies.Add(newEnemy);

        }
        */

        if(PlayerHealthController.instance.gameObject.activeSelf)
        {
            if(currentWave < waves.Count)
            {
                waveCounter -= Time.deltaTime;

                if(waveCounter <= 0)
                {
                    GoToNextWave();
                }

                spawnCounter -= Time.deltaTime;

                if(spawnCounter <=  0)
                {
                    spawnCounter = waves[currentWave].timeBetweenSpawns;

                    GameObject newEnemy = Instantiate(waves[currentWave].enemyToSpawn, SelectSpawnPoint(), Quaternion.identity);

                    spawnedEnemies.Add(newEnemy);
                }
            }
        }

        transform.position = target.position;

        int checkTarget = enemyToCheck + checkPerFrame;

        while(enemyToCheck < checkTarget)
        {
            if (enemyToCheck < spawnedEnemies.Count)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    if(Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance )
                    {
                        Destroy(spawnedEnemies[enemyToCheck]);

                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--;
                    } else
                    {
                        enemyToCheck++;
                    }
                } else
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    checkTarget--;
                }
            } else
            {
                enemyToCheck = 0;
                checkTarget = 0;
            }
        }
    }

    public void GoToNextWave()
    {
        currentWave++;

        if(currentWave >= waves.Count)
        {
            currentWave = waves.Count - 1;
        }

        waveCounter = waves[currentWave].waveLength;
        spawnCounter = waves[currentWave].timeBetweenSpawns;
    }
}

[System.Serializable]

public class WaveInfo
{
    public GameObject enemyToSpawn;
    public float waveLength = 10f;
    public float timeBetweenSpawns = 1f;
}
