using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn; // Inimigo geral, pode ser removido se s� usar waves
    public float timeTopSpawn; // N�o usado no sistema de ondas atual, pode ser removido
    private float spawnCounter;

    public Transform minSpawn, maxSpawn;

    private Transform target;

    private float despawnDistance;

    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Inimigos atualmente na cena

    public int checkPerFrame; // Para despawn
    private int enemyToCheck; // Para despawn

    public List<WaveInfo> waves;

    private int currentWave;
    private float waveCounter;

    private bool allWavesInitiated = false; // Flag para indicar se todas as ondas foram iniciadas

    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        // L�gica de sele��o de ponto de spawn
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
        }
        else
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
        // Encontra o player para seguir
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("PlayerController n�o encontrado! O Spawner n�o poder� seguir o player.");
        }

        // Calcula a dist�ncia de despawn
        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;

        currentWave = -1; // Come�a antes da primeira onda
        allWavesInitiated = false; // Reseta a flag

        // Inicializa os timers de todas as waves
        for (int i = 0; i < waves.Count; i++)
        {
            waves[i].spawnTimer = waves[i].timeBetweenSpawns;
        }

        GoToNextWave(); // Inicia a primeira onda
    }

    void Update()
    {
        // Certifica-se de que o player est� ativo antes de continuar
        if (PlayerHealthController.instance != null && PlayerHealthController.instance.gameObject.activeSelf)
        {
            // L�gica de spawn das ondas
            if (!allWavesInitiated) // Se ainda h� ondas a serem iniciadas
            {
                waveCounter -= Time.deltaTime;

                if (waveCounter <= 0) // Dura��o da onda atual de spawn terminou
                {
                    GoToNextWave(); // Tenta ir para a pr�xima onda
                }


                if (currentWave >= 0 && currentWave < waves.Count)
                {
                    // Para cada wave de 0 at� a atual
                    for (int i = 0; i <= currentWave; i++)
                    {
                        // Cada wave tem seu pr�prio timer
                        waves[i].spawnTimer -= Time.deltaTime;

                        if (waves[i].spawnTimer <= 0)
                        {
                            waves[i].spawnTimer = waves[i].timeBetweenSpawns;

                            GameObject newEnemy = Instantiate(waves[i].enemyToSpawn, SelectSpawnPoint(), Quaternion.identity);
                            spawnedEnemies.Add(newEnemy);
                        }
                    }
                }
            }
            else // Todas as ondas j� iniciaram seus spawns
            {
                // Verifica a condi��o de fim de fase: n�o h� mais inimigos spawnados em cena
                if (spawnedEnemies.Count == 0)
                {
                    Debug.Log("Todos os inimigos derrotados. Fase Completa!");
                    // Chama o LevelManager para avisar que a fase terminou
                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.LevelCompleted();
                        // Desabilita este spawner para que n�o fa�a mais nada ap�s a conclus�o da fase
                        enabled = false; // Desativa o script para parar o Update
                    }
                    else
                    {
                        Debug.LogError("LevelManager n�o encontrado! N�o foi poss�vel completar a fase.");
                    }
                }
            }
        }

        // Move o spawner com o player
        if (target != null)
        {
            transform.position = target.position;
        }

        // L�gica de Despawn
        int checkTarget = enemyToCheck + checkPerFrame;

        while (enemyToCheck < checkTarget)
        {
            if (enemyToCheck < spawnedEnemies.Count)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    // Verifica dist�ncia para despawn
                    if (Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                    {
                        Destroy(spawnedEnemies[enemyToCheck]);
                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--; // Ajusta o alvo de verifica��o porque o tamanho da lista diminuiu
                    }
                    else
                    {
                        enemyToCheck++;
                    }
                }
                else // Inimigo foi destru�do por outra forma (ex: pelo jogador)
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    checkTarget--; // Ajusta o alvo de verifica��o
                }
            }
            else // Chegou ao final da lista
            {
                enemyToCheck = 0; // Reseta para o pr�ximo frame
                checkTarget = 0; // Sai do loop
            }
        }
    }

    public void GoToNextWave()
    {
        currentWave++;

        if (currentWave < waves.Count)
        {
            waveCounter = waves[currentWave].waveLength;

           
            waves[currentWave].spawnTimer = waves[currentWave].timeBetweenSpawns;

            Debug.Log($"Iniciando Onda {currentWave + 1} de {waves.Count}");
        }
        else
        {
            // Todas as ondas foram processadas para spawn
            allWavesInitiated = true;
            Debug.Log("Todas as ondas foram iniciadas. Aguardando que os inimigos sejam derrotados.");
        }
    }
}

[System.Serializable]
public class WaveInfo
{
    public GameObject enemyToSpawn;
    public float waveLength = 10f; // Dura��o total para spawnar inimigos nesta onda
    public float timeBetweenSpawns = 1f;

    [HideInInspector]
    public float spawnTimer; // Timer individual para cada wave
}