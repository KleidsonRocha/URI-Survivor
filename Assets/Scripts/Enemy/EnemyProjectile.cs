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


    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();



        // Sistema de crescimento visual
        targetSize = transform.localScale;
        transform.localScale = Vector3.zero;

        // Destruir após o tempo limite
        Destroy(gameObject, lifeTime);


    }

    void Update()
    {
        // Animação de crescimento
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, growSpeed * Time.deltaTime);

        // Movimento do projétil
        if (direction != Vector3.zero && !hasHitTarget && rb != null)
        {
            rb.linearVelocity = direction * speed;


        }
    }

    public void Initialize(Vector3 shootDirection, float projectileDamage, float projectileSpeed)
    {
        direction = shootDirection.normalized;
        damage = projectileDamage;
        speed = projectileSpeed;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player") && !hasHitTarget)
        {
            hasHitTarget = true;



            // Causar dano ao jogador
            PlayerHealthController.instance.TakeDamage(damage);

            // Destruir o projétil
            DestroyProjectile();
        }
        else if (collision.CompareTag("Wall"))
        {


            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }


        Destroy(gameObject);
    }
}