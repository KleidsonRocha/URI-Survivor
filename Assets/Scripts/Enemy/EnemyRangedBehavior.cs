using UnityEngine;

public class EnemyRangedBehavior : MonoBehaviour
{
    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public float projectileSpeed = 3f;
    public float projectileDamage = 1f;

    [Header("Behavior Settings")]
    public float keepDistanceRange = 3f;
    public bool shouldMaintainDistance = true;

    [Header("Debug")]
    public bool enableDebugLogs = false;

    private Transform target;
    private float lastAttackTime;
    private EnemyController enemyController;
    private float originalStopDistance;
    private bool isInRangedMode = false;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
        enemyController = GetComponent<EnemyController>();

        if (enemyController != null)
        {
            originalStopDistance = enemyController.stopDistance;
        }

        if (firePoint == null)
        {
            firePoint = transform;
            if (enableDebugLogs) Debug.LogWarning($"{gameObject.name}: firePoint n�o definido, usando transform do objeto.");
        }

        if (projectilePrefab == null)
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: projectilePrefab n�o est� definido!");
        }

        if (enableDebugLogs) Debug.Log($"{gameObject.name}: EnemyRangedBehavior iniciado. AttackRange: {attackRange}, Cooldown: {attackCooldown}");
    }

    void Update()
    {
        if (target == null || enemyController == null || enemyController.IsDead())
            return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Debug da dist�ncia
        if (enableDebugLogs && Time.frameCount % 60 == 0) // Log a cada segundo aproximadamente
        {
            Debug.Log($"{gameObject.name}: Dist�ncia para o alvo: {distanceToTarget:F2}, Range: {attackRange}, InRangedMode: {isInRangedMode}");
        }

        // Verificar se deve entrar em modo ranged
        if (distanceToTarget <= attackRange && !isInRangedMode)
        {
            EnterRangedMode();
        }
        else if (distanceToTarget > attackRange && isInRangedMode)
        {
            ExitRangedMode();
        }

        // L�gica de ataque � dist�ncia
        if (isInRangedMode)
        {
            HandleRangedBehavior(distanceToTarget);
        }
    }

    private void EnterRangedMode()
    {
        isInRangedMode = true;

        if (enableDebugLogs) Debug.Log($"{gameObject.name}: Entrando em modo ranged!");

        if (shouldMaintainDistance && enemyController != null)
        {
            enemyController.stopDistance = keepDistanceRange;
            if (enableDebugLogs) Debug.Log($"{gameObject.name}: StopDistance alterado para: {keepDistanceRange}");
        }
    }

    private void ExitRangedMode()
    {
        isInRangedMode = false;

        if (enableDebugLogs) Debug.Log($"{gameObject.name}: Saindo do modo ranged!");

        if (enemyController != null)
        {
            enemyController.stopDistance = originalStopDistance;
        }
    }

    private void HandleRangedBehavior(float distanceToTarget)
    {
        // Debug do cooldown
        float timeSinceLastAttack = Time.time - lastAttackTime;
        bool canAttack = timeSinceLastAttack >= attackCooldown;

        if (enableDebugLogs && Time.frameCount % 30 == 0) // Log mais frequente para debug
        {
            Debug.Log($"{gameObject.name}: TimeSinceLastAttack: {timeSinceLastAttack:F2}, CanAttack: {canAttack}, Cooldown: {attackCooldown}");
        }

        // Verificar se pode atacar
        if (canAttack)
        {
            bool hasLineOfSight = HasClearLineOfSight();

            if (enableDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Tentando atacar! LineOfSight: {hasLineOfSight}");
            }

            if (hasLineOfSight)
            {
                ShootProjectile();
                lastAttackTime = Time.time;
            }
        }

        // L�gica de manter dist�ncia
        if (shouldMaintainDistance && distanceToTarget < keepDistanceRange && enemyController != null)
        {
            Vector2 directionAway = (transform.position - target.position).normalized;
            enemyController.theRB.linearVelocity = directionAway * enemyController.moveSpeed;

            if (enableDebugLogs && Time.frameCount % 60 == 0)
            {
                Debug.Log($"{gameObject.name}: Mantendo dist�ncia - movendo para longe do player");
            }
        }
    }

    private bool HasClearLineOfSight()
    {
        if (target == null || firePoint == null) return false;

        Vector2 directionToTarget = (target.position - firePoint.position).normalized;
        float distanceToTarget = Vector2.Distance(firePoint.position, target.position);

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, directionToTarget, distanceToTarget);

        // Linha de vis�o clara se:
        // 1. N�o atingiu nada (espa�o livre)
        // 2. Atingiu o player (alvo v�lido)
        // 3. Atingiu outro inimigo (pode atirar atrav�s)
        bool hasLineOfSight = hit.collider == null ||
                             hit.collider.CompareTag("Player") ||
                             hit.collider.CompareTag("Enemy");

        if (enableDebugLogs && !hasLineOfSight)
        {
            Debug.Log($"{gameObject.name}: Linha de vis�o bloqueada por: {hit.collider?.name}");
        }

        return hasLineOfSight;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null)
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: projectilePrefab � null!");
            return;
        }

        if (target == null || firePoint == null)
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: target ou firePoint � null!");
            return;
        }

        // Calcular dire��o para o alvo
        Vector3 directionToTarget = (target.position - firePoint.position).normalized;

        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name}: ATIRANDO! Posi��o: {firePoint.position}, Dire��o: {directionToTarget}, Prefab: {projectilePrefab.name}");
        }

        // Instanciar o proj�til
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        if (projectile == null)
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: Falha ao instanciar proj�til!");
            return;
        }

        // Configurar o proj�til
        EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(directionToTarget, projectileDamage, projectileSpeed);
            if (enableDebugLogs) Debug.Log($"{gameObject.name}: Proj�til configurado com sucesso!");
        }
        else
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: EnemyProjectile script n�o encontrado no prefab!");
        }

        // Rotacionar o proj�til na dire��o do movimento
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    // Visualiza��o no editor
    private void OnDrawGizmosSelected()
    {
        // Desenhar range de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Desenhar dist�ncia de manuten��o
        if (shouldMaintainDistance)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, keepDistanceRange);
        }

        // Desenhar linha para o firePoint
        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, firePoint.position);
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }

        // Desenhar linha de vis�o para o target
        if (target != null && firePoint != null)
        {
            Gizmos.color = HasClearLineOfSight() ? Color.green : Color.red;
            Gizmos.DrawLine(firePoint.position, target.position);
        }
    }
}