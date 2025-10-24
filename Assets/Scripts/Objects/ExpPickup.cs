using UnityEngine;

public class ExpPickup : MonoBehaviour
{
    public int expValue;

    private bool movingToPlayer = false; // Come�a como false por padr�o
    public float moveSpeed = 5f; // Velocidade base do orbe
    public float magnetizeSpeedMultiplier = 2.5f; // Multiplicador de velocidade quando magnetizado

    public float timeBetweenChecks = .2f;
    private float checkCounter;

    private Transform playerTransform; // Usar Transform para movimento direto
    private PlayerController playerController; // Para acessar pickupRange

    void Start()
    {
        // Garante que o PlayerController.Instance existe antes de tentar acess�-lo
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
            playerController = PlayerController.Instance; // Para acessar o pickupRange
        }
        else
        {
            Debug.LogWarning("ExpPickup: PlayerController.Instance n�o encontrado. O script ExpPickup ser� desabilitado.");
            enabled = false; // Desabilita o script se n�o encontrar o player
            return;
        }

        checkCounter = timeBetweenChecks; // Inicia o contador para a primeira verifica��o
    }

    void Update()
    {
        if (playerTransform == null) return; // Se o player sumiu, n�o fa�a nada

        if (movingToPlayer == true)
        {
            // Move em dire��o ao player usando a velocidade base e o multiplicador de magnetismo
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, (moveSpeed * magnetizeSpeedMultiplier) * Time.deltaTime);
        }
        else
        {
            // L�gica original para verificar se o player entrou no range de pickup
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

    // NOVO M�TODO: Chamado pelo PickupAllCoin para ativar o magnetismo
    public void StartMagnetizing()
    {
        movingToPlayer = true;
        // Opcional: Voc� pode querer dar um boost extra ou ajustar a velocidade aqui especificamente para o magnetismo do "Coletar Tudo"
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