using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Enemy Settings")]
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float stopDistance = 0.5f;
    private Transform target;

    [Header("Combat Settings")]
    public float damage;
    public float hitWaitTime = 1f;
    private float hitCounter;

    public float health = 5f;

    [Header("Knockback Settings")]
    public float knockBackTime = .5f;
    private float knockBackCounter;

    [Header("Damage Detection System")]
    public bool useDamageDetection = true; // Toggle para ativar/desativar

    // Flag do sistema de detecção - Similar ao wasMoving do player
    private bool hasTakenDamage = false;
    private bool isDead = false;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    void FixedUpdate()
    {
        if (target == null || isDead)
        {
            theRB.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > stopDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            theRB.linearVelocity = direction * moveSpeed;
        }
        else
        {
            theRB.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;

            if (moveSpeed > 0)
            {
                moveSpeed = -moveSpeed * 2f;
            }

            if (knockBackCounter <= 0)
            {
                moveSpeed = Mathf.Abs(moveSpeed * .5f);
            }
        }

        if (hitCounter > 0f)
        {
            hitCounter -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);
            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        // SISTEMA DE DETECÇÃO DE DANO - Similar ao isMoving do player
        if (useDamageDetection && !hasTakenDamage)
        {
            hasTakenDamage = true; // Ativa a flag (permanece ativa até morrer)
            Debug.Log($"{gameObject.name} tomou dano pela primeira vez!");
        }

        if (health <= 0)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} morreu!");
            Destroy(gameObject);
        }

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);

        if (shouldKnockBack == true)
        {
            knockBackCounter = knockBackTime;
        }
    }

    // Métodos públicos para outros scripts consultarem o estado
    public bool HasTakenDamage()
    {
        return hasTakenDamage;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // Método para resetar o estado (útil para testing ou respawn)
    public void ResetDamageState()
    {
        if (useDamageDetection)
        {
            hasTakenDamage = false;
            isDead = false;
        }
    }
}