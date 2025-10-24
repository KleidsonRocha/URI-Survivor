using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public int expValue;

    private bool movingToPlayer = false; // Começa como false por padrão
    public float moveSpeed = 5f; // Velocidade base do orbe
    public float magnetizeSpeedMultiplier = 2.5f; // Multiplicador de velocidade quando magnetizado

    public float timeBetweenChecks = .2f;
    private float checkCounter;

    private Transform playerTransform; // Usar Transform para movimento direto
    private PlayerController playerController; // Para acessar pickupRange

    void Start()
    {
        // Garante que o PlayerController.Instance existe antes de tentar acessá-lo
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
            playerController = PlayerController.Instance; // Para acessar o pickupRange
        }
        else
        {
            Debug.LogWarning("ExpPickup: PlayerController.Instance não encontrado. O script ExpPickup será desabilitado.");
            enabled = false; // Desabilita o script se não encontrar o player
            return;
        }

        checkCounter = timeBetweenChecks; // Inicia o contador para a primeira verificação
    }

    void Update()
    {
        if (playerTransform == null) return; // Se o player sumiu, não faça nada

        if (movingToPlayer == true)
        {
            // Move em direção ao player usando a velocidade base e o multiplicador de magnetismo
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, (moveSpeed * magnetizeSpeedMultiplier) * Time.deltaTime);
        }
        else
        {
            // Lógica original para verificar se o player entrou no range de pickup
            checkCounter -= Time.deltaTime;
            if (checkCounter <= 0)
            {
                checkCounter = timeBetweenChecks;

                if (playerController != null && Vector3.Distance(transform.position, playerTransform.position) < playerController.pickupRange)
                {
                    movingToPlayer = true;
                    // Opcional: Aumentar a velocidade base do orbe quando ativado por pickupRange
                    // moveSpeed += playerController.moveSpeed; 
                }
            }
        }
    }

    // NOVO MÉTODO: Chamado pelo PickupAllCoin para ativar o magnetismo
    public void StartMagnetizing()
    {
        movingToPlayer = true;
        // Opcional: Você pode querer dar um boost extra ou ajustar a velocidade aqui especificamente para o magnetismo do "Coletar Tudo"
        // Por exemplo: moveSpeed = 10f; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Use CompareTag para melhor performance
        {
            if (ExperienceLevelController.instance != null)
            {
                ExperienceLevelController.instance.GetExp(expValue);
            }
            Destroy(gameObject);
        }
    }
}