using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Define os tipos de movimento dos inimigos para serem usados nas ondas
public enum EnemyMovementType
{
    Normal,
    Dashing,
    // Adicione mais tipos de movimento aqui, se necessário
}

public enum WaveContentType
{
    RegularEnemies, // A onda spawna inimigos regulares definidos por enemyToSpawn
    BossEncounter   // A onda spawna um único bossPrefab
}

[System.Serializable]
public class WaveInfo
{
    [Header("Tipo de Conteúdo da Onda")]
    public WaveContentType contentType = WaveContentType.RegularEnemies;

    [Header("Configurações de Inimigos Regulares (se 'RegularEnemies')")]
    public GameObject enemyToSpawn;
    public float waveLength = 10f; 
    public float timeBetweenSpawns = 1f; 

    [Header("Configurações de Encontro com Chefe (se 'BossEncounter')")]
    public GameObject bossPrefab; 
    public Vector3 bossSpawnOffset = new Vector3(0, 5, 0);

    [Header("Especificações de Inimigo para Esta Onda (aplica-se a 'enemyToSpawn' ou 'bossPrefab')")]
    public EnemyMovementType movementType = EnemyMovementType.Normal;
    public float dashSpeedMultiplier = 2f; 
    public float dashDuration = 1f;
    public float dashCooldown = 3f; 

    [HideInInspector]
    public float spawnTimer;
}

public class EnemySpawner : MonoBehaviour
{
    // Removido: public GameObject enemyToSpawn; // Não é mais necessário aqui.
    // Removido: public float timeTopSpawn; // Não é mais necessário.
    // Removido: private float spawnCounter; // Pode ser removido, não é mais usado diretamente.

    public Transform minSpawn, maxSpawn;

    private Transform target; 

    private float despawnDistance; 

    private List<GameObject> spawnedEnemies = new List<GameObject>(); 

    public int checkPerFrame;
    private int enemyToCheck; 

    public List<WaveInfo> waves;

    private int currentWave; 
    private float waveCounter; 

    private bool allWavesInitiated = false; 
    private bool allWavesCompleted = false; 


    private GameObject currentBossGameObject;

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
        // Encontra o player para seguir e definir como target
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("PlayerController não encontrado! O Spawner não poderá seguir o player.");
        }

        // Calcula a distância de despawn com base nos limites da câmera/mapa
        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;

        currentWave = -1; 
        allWavesInitiated = false;
        allWavesCompleted = false;
        currentBossGameObject = null; 

        // Inicializa os timers de spawn para todas as waves de inimigos regulares
        for (int i = 0; i < waves.Count; i++)
        {
            if (waves[i].contentType == WaveContentType.RegularEnemies)
            {
                waves[i].spawnTimer = waves[i].timeBetweenSpawns;
            }
        }

        GoToNextWave();
    }

    void Update()
    {
      
        if (PlayerHealthController.instance != null && PlayerHealthController.instance.gameObject.activeSelf && !allWavesCompleted)
        {

            if (!allWavesInitiated) 
            {
 
                if (currentWave >= 0 && currentWave < waves.Count && waves[currentWave].contentType == WaveContentType.RegularEnemies)
                {
                    waveCounter -= Time.deltaTime; 
                    if (waveCounter <= 0)
                    {
                        GoToNextWave();
                    }
                }
               
                else if (currentWave >= 0 && currentWave < waves.Count && waves[currentWave].contentType == WaveContentType.BossEncounter)
                {
                    if (currentBossGameObject == null) 
                    {
                        Debug.Log($"Boss da Onda {currentWave + 1} derrotado!");
                        GoToNextWave(); // Avança para a próxima onda
                    }
                }

                // Lógica de spawn para ondas de inimigos regulares ativas
                if (currentWave >= 0 && currentWave < waves.Count)
                {
                    for (int i = 0; i <= currentWave; i++)
                    {

                        if (waves[i].contentType == WaveContentType.RegularEnemies)
                        {
                            waves[i].spawnTimer -= Time.deltaTime;

                            if (waves[i].spawnTimer <= 0)
                            {
                                waves[i].spawnTimer = waves[i].timeBetweenSpawns; 

                                GameObject newEnemyGO = Instantiate(waves[i].enemyToSpawn, SelectSpawnPoint(), Quaternion.identity);
                                spawnedEnemies.Add(newEnemyGO);


                                EnemyController newEnemyController = newEnemyGO.GetComponent<EnemyController>();
                                if (newEnemyController != null)
                                {
                                    newEnemyController.currentMovementType = waves[i].movementType;
                                    newEnemyController.dashSpeedMultiplier = waves[i].dashSpeedMultiplier;
                                    newEnemyController.dashDuration = waves[i].dashDuration;
                                    newEnemyController.dashCooldown = waves[i].dashCooldown;
                                    newEnemyController.isBoss = false; // Garante que não é um chefe
                                }
                                else
                                {
                                    Debug.LogWarning($"O inimigo '{newEnemyGO.name}' da onda {i + 1} não possui um EnemyController.");
                                }
                            }
                        }
                      
                    }
                }
            }
            else 
            {
              
                for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
                {
                    if (spawnedEnemies[i] == null)
                    {
                        spawnedEnemies.RemoveAt(i);
                    }
                }

               
                bool noRegularEnemiesLeft = (spawnedEnemies.Count == 0);
                bool bossDefeatedOrNone = (currentBossGameObject == null);

                if (noRegularEnemiesLeft && bossDefeatedOrNone)
                {
                    Debug.Log("Todos os inimigos e bosses derrotados. Fase Completa!");
                    allWavesCompleted = true; 

                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.LevelCompleted();
                        enabled = false; 
                    }
                    else
                    {
                        Debug.LogError("LevelManager não encontrado! Não foi possível completar a fase.");
                    }
                }
            }
        }
        else if (allWavesCompleted) 
        {
            return;
        }

      
        if (target != null)
        {
            transform.position = target.position;
        }

      
        if (spawnedEnemies != null && spawnedEnemies.Count > 0)
        {
            int originalCount = spawnedEnemies.Count;
            int enemiesCheckedThisFrame = 0;

            while (enemiesCheckedThisFrame < checkPerFrame && enemyToCheck < originalCount)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    if (Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                    {
                        Destroy(spawnedEnemies[enemyToCheck]);
                        spawnedEnemies.RemoveAt(enemyToCheck);
                        originalCount--;
                    }
                    else
                    {
                        enemyToCheck++;
                    }
                }
                else
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    originalCount--;
                }
                enemiesCheckedThisFrame++;
            }

            if (enemyToCheck >= originalCount)
            {
                enemyToCheck = 0;
            }
        }
        else
        {
            enemyToCheck = 0;
        }
    }

    public void GoToNextWave()
    {
        currentWave++;
        currentBossGameObject = null;

        if (currentWave < waves.Count)
        {
            WaveInfo nextWave = waves[currentWave];

            if (nextWave.contentType == WaveContentType.RegularEnemies)
            {
                waveCounter = nextWave.waveLength;
                nextWave.spawnTimer = nextWave.timeBetweenSpawns;
                Debug.Log($"Iniciando Onda {currentWave + 1} de {waves.Count} (Inimigos Regulares). Tipo de Movimento: {nextWave.movementType}");
            }
            else if (nextWave.contentType == WaveContentType.BossEncounter)
            {
                if (nextWave.bossPrefab != null)
                {
                    // Spawna o chefe em uma posição relativa ao jogador/spawner
                    Vector3 bossSpawnPos = target.position + nextWave.bossSpawnOffset;
                    GameObject newBossGO = Instantiate(nextWave.bossPrefab, bossSpawnPos, Quaternion.identity);
                    currentBossGameObject = newBossGO; // Guarda a referência do chefe


                    EnemyController bossController = newBossGO.GetComponent<EnemyController>();
                    if (bossController != null)
                    {
                        bossController.currentMovementType = nextWave.movementType;
                        bossController.dashSpeedMultiplier = nextWave.dashSpeedMultiplier;
                        bossController.dashDuration = nextWave.dashDuration;
                        bossController.dashCooldown = nextWave.dashCooldown;
                        bossController.isBoss = true; // Marca este inimigo como um chefe
                    }
                    else
                    {
                        Debug.LogWarning($"O Boss '{newBossGO.name}' da onda {currentWave + 1} não possui um EnemyController.");
                    }
                    Debug.Log($"Iniciando Onda {currentWave + 1} de {waves.Count} (Encontro com Chefe). Chefe: {newBossGO.name}");
                }
                else
                {
                    Debug.LogError($"Wave {currentWave + 1} é do tipo Boss Encounter, mas não tem um Boss Prefab atribuído! Avançando para a próxima onda.");
                    // Avança para a próxima onda para evitar travamento se não houver chefe configurado
                    GoToNextWave();
                }
            }
        }
        else
        {
            allWavesInitiated = true; // Todas as ondas foram iniciadas
            Debug.Log("Todas as ondas foram iniciadas. Aguardando que os inimigos em cena (e o boss, se houver) sejam derrotados.");
        }
    }
}