using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 5f;
    public float damage = 1f;
    public float lifeTime = 3f;
    public bool shouldKnockBack = false;

    [Header("Visual Effects")]
    public float growSpeed = 10f;
    private Vector3 targetSize;
    private Vector3 direction;
    private bool hasHitTarget = false;

    [Header("Debug")]
    public bool enableDebugLogs = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            if (enableDebugLogs) Debug.LogError($"{gameObject.name}: Rigidbody2D não encontrado!");
        }

        // Sistema de crescimento visual
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;

        // Destruir após o tempo limite
        Destroy(gameObject, lifeTime);

        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Projétil criado! Speed: {speed}, Damage: {damage}, LifeTime: {lifeTime}");
        }
    }

    void Update()
    {
        // Animação de crescimento
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);

        // Movimento do projétil
        if (direction != Vector3.zero && !hasHitTarget && rb != null)
        {
            rb.linearVelocity = direction * speed;

            // Debug do movimento (apenas ocasionalmente)
            if (enableDebugLogs && Time.frameCount % 60 == 0)
            {
                Debug.Log($"{gameObject.name}: Movendo na direção {direction} com velocidade {speed}. Posição atual: {transform.position}");
            }
        }
    }

    public void Initialize(Vector3 shootDirection, float projectileDamage, float projectileSpeed)
    {
        direction = shootDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;

        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Inicializado! Direção: {direction}, Dano: {damage}, Velocidade: {speed}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Colidiu com {collision.name} (Tag: {collision.tag})");
        }

        if (collision.CompareTag("Player") && !hasHitTarget)
        {
            hasHitTarget = true;

            if (enableDebugLogs)
            {
                Debug.Log($"{gameObject.name}: ACERTOU O PLAYER! Causando {damage} de dano.");
            }

            // Causar dano ao jogador
            PlayerHealthController.instance.TakeDamage(damage);

            // Destruir o projétil
            DestroyProjectile();
        }
        else if (collision.CompareTag("Wall"))
        {
            if (enableDebugLogs)
            {
                Debug.Log($"{gameObject.name}: Colidiu com parede/obstáculo: {collision.name}");
            }

            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name}: Projétil destruído!");
        }

        Destroy(gameObject);
    }
}