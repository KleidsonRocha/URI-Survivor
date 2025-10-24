using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Enemy Settings")]
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float stopDistance = 0.5f;
    public int expToGive = 1;


    [Header("Item Drops")]
    public GameObject pickupAllCoinPrefab; 
    public float pickupAllCoinDropRate = 0.1f; 

    public int coinValue = 1;
    public float coinDropRate = .5f;
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


    [Header("Animation")]
    public Animator animator;

  
    private bool hasTakenDamage = false;
    private bool isDead = false;


    [HideInInspector] public EnemyMovementType currentMovementType = EnemyMovementType.Normal;
    [HideInInspector] public float dashSpeedMultiplier = 2f; 
    [HideInInspector] public float dashDuration = 1f; 
    [HideInInspector] public float dashCooldown = 3f;

    private float dashTimer;
    private float dashCooldownTimer;
    private bool isDashing = false;
    private bool canDash = true; 

 
    [HideInInspector] public bool isBoss = false;


    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }


        if (currentMovementType == EnemyMovementType.Dashing)
        {
            dashCooldownTimer = Random.Range(0f, dashCooldown); 
        }
    }

    void FixedUpdate()
    {

        if (target == null || isDead || !PlayerController.Instance.gameObject.activeSelf)
        {
            theRB.linearVelocity = Vector2.zero;
            return;
        }


        if (knockBackCounter > 0)
        {
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        float currentSpeed = moveSpeed;

        if (currentMovementType == EnemyMovementType.Dashing)
        {
            // Lógica de dash
            if (isDashing)
            {
                currentSpeed *= dashSpeedMultiplier; 
                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0)
                {
                    isDashing = false; 
                    dashCooldownTimer = dashCooldown; 
                }
            }
            else
            {
                dashCooldownTimer -= Time.fixedDeltaTime;
                if (dashCooldownTimer <= 0 && canDash) 
                {
                    isDashing = true; 
                    dashTimer = dashDuration; 

                }
            }
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > stopDistance)
        {
            theRB.linearVelocity = direction * currentSpeed;
        }
        else
        {
            theRB.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (PlayerController.Instance.gameObject.activeSelf == true)
        {
            if (knockBackCounter > 0)
            {

                if (isBoss)
                {
                    knockBackCounter = 0; // Se for um chefe, cancela o knockback. Chefes são imunes.

                }
                else
                {
                    knockBackCounter -= Time.deltaTime;

                    // Lógica de movimento reverso durante o knockback 
                    if (moveSpeed > 0)
                    {
                        moveSpeed = -Mathf.Abs(moveSpeed) * 2f; 
                    }

                    if (knockBackCounter <= 0)
                    {
                        moveSpeed = Mathf.Abs(moveSpeed * .5f); 
                    }
                }
            }

            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime; 
            }
        }
        else
        {
            theRB.linearVelocity = Vector2.zero; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall"))
        {
            return;
        }


        if (collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);
            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;


        if (useDamageDetection && !hasTakenDamage)
        {
            hasTakenDamage = true;
            if (animator != null)
            {
                animator.SetBool("hasTakenDamage", true); 
            }
        }

        if (health <= 0)
        {
            isDead = true; 

            Destroy(gameObject);

            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if(Random.value <= coinDropRate)
            {
                CoinController.instance.DropCooin(transform.position, coinValue);
            }

            if (Random.value <= pickupAllCoinDropRate)
            {
    
                Instantiate(pickupAllCoinPrefab, transform.position + new Vector3(.2f, .1f, 0f), Quaternion.identity);
                Debug.Log("Inimigo dropou um 'Pickup All Coin'!");
            }
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


    public bool HasTakenDamage()
    {
        return hasTakenDamage;
    }

    public bool IsDead()
    {
        return isDead;
    }


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