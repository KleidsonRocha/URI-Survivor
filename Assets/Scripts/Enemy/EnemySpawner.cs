using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Importar para carregar cenas (mas LevelManager fará isso)

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn; // Inimigo geral, pode ser removido se só usar waves
    public float timeTopSpawn; // Não usado no sistema de ondas atual, pode ser removido
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

    private bool allWavesInitiated = false; // Nova flag para indicar se todas as ondas foram iniciadas


    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        // Lógica de seleção de ponto de spawn (conforme seu código)
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
        // Encontra o player para seguir (assumindo que PlayerController está no player)
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("PlayerController não encontrado! O Spawner não poderá seguir o player.");
            // Se PlayerController for essencial, você pode desabilitar o spawner ou exibir uma mensagem de erro fatal aqui.
        }

        // Calcula a distância de despawn
        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;

        currentWave = -1; // Começa antes da primeira onda
        allWavesInitiated = false; // Reseta a flag
        GoToNextWave(); // Inicia a primeira onda
    }

    void Update()
    {
        // Certifica-se de que o player está ativo antes de continuar a lógica do spawner
        // (Assumindo que PlayerHealthController.instance existe e controla a vida do player)
        if (PlayerHealthController.instance != null && PlayerHealthController.instance.gameObject.activeSelf)
        {
            // Lógica de spawn das ondas
            if (!allWavesInitiated) // Se ainda há ondas a serem iniciadas
            {
                waveCounter -= Time.deltaTime;

                if (waveCounter <= 0) // Duração da onda atual de spawn terminou
                {
                    GoToNextWave(); // Tenta ir para a próxima onda
                }

                // Se ainda estamos em uma onda válida para spawn (não todas as ondas completadas ainda)
                if (currentWave < waves.Count)
                {
                    spawnCounter -= Time.deltaTime;

                    if (spawnCounter <= 0)
                    {
                        spawnCounter = waves[currentWave].timeBetweenSpawns;

                        GameObject newEnemy = Instantiate(waves[currentWave].enemyToSpawn, SelectSpawnPoint(), Quaternion.identity);
                        spawnedEnemies.Add(newEnemy); // Adiciona o inimigo à lista de inimigos ativos
                    }
                }
            }
            else // Todas as ondas já iniciaram seus spawns
            {
                // Verifica a condição de fim de fase: não há mais inimigos spawnados em cena
                // A lista `spawnedEnemies` já se auto-limpa de inimigos destruídos/despawnados
                if (spawnedEnemies.Count == 0)
                {
                    Debug.Log("Todos os inimigos derrotados. Fase Completa!");
                    // Chama o LevelManager para avisar que a fase terminou
                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.LevelCompleted();
                        // Desabilita este spawner para que não faça mais nada após a conclusão da fase
                        enabled = false; // Desativa o script para parar o Update
                    }
                    else
                    {
                        Debug.LogError("LevelManager não encontrado! Não foi possível completar a fase. Certifique-se de que o LevelManager está na cena de carregamento inicial.");
                    }
                }
            }
        }

        // Move o spawner com o player
        if (target != null)
        {
            transform.position = target.position;
        }

        // Lógica de Despawn (conforme seu código)
        int checkTarget = enemyToCheck + checkPerFrame;

        while (enemyToCheck < checkTarget)
        {
            if (enemyToCheck < spawnedEnemies.Count)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    // Verifica distância para despawn
                    if (Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                    {
                        Destroy(spawnedEnemies[enemyToCheck]);
                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--; // Ajusta o alvo de verificação porque o tamanho da lista diminuiu
                    }
                    else
                    {
                        enemyToCheck++;
                    }
                }
                else // Inimigo foi destruído por outra forma (ex: pelo jogador)
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    checkTarget--; // Ajusta o alvo de verificação
                }
            }
            else // Chegou ao final da lista
            {
                enemyToCheck = 0; // Reseta para o próximo frame
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
            spawnCounter = waves[currentWave].timeBetweenSpawns;
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

// WaveInfo permanece o mesmo
[System.Serializable]
public class WaveInfo
{
    public GameObject enemyToSpawn;
    public float waveLength = 10f; // Duração total para spawnar inimigos nesta onda
    public float timeBetweenSpawns = 1f;
}