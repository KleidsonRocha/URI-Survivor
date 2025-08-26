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
    public bool useDamageDetection = true;

    // ADICIONE ESTA LINHA - Referência ao Animator
    [Header("Animation")]
    public Animator animator;

    // Flag do sistema de detecção
    private bool hasTakenDamage = false;
    private bool isDead = false;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
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

        // SISTEMA DE DETECÇÃO DE DANO - MODIFICADO
        if (useDamageDetection && !hasTakenDamage)
        {
            hasTakenDamage = true;

            if (animator != null)
            {
                animator.SetBool("hasTakenDamage", true);
            }

            //Debug.Log($"{gameObject.name} tomou dano pela primeira vez!");
        }

        if (health <= 0)
        {
            isDead = true;

            // OPCIONAL - Atualizar animator para estado de morte
            //if (animator != null)
            //{
            //    animator.SetBool("isDead", true);
            //}

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

            // Reseta também o animator
            if (animator != null)
            {
                animator.SetBool("hasTakenDamage", false);
                animator.SetBool("isDead", false);
            }
        }
    }
}