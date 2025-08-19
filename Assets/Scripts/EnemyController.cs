using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float stopDistance = 0.5f;
    private Transform target;

    public float damage;
    public float hitWaitTime = 1f;
    private float hitCounter;

    public float health = 5f;

    public float knockBackTime = .5f;
    private float knockBackCounter;



    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    void FixedUpdate() // Use FixedUpdate para física
    {
        if (target == null)
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

    void Update() // Para logicas a parte
    {
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;

            if(moveSpeed > 0)
            {
                moveSpeed = -moveSpeed * 2f;
            }

            if(knockBackCounter <=0)
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
        if(collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);

            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage( float damakeToTake)
    {
        health -= damakeToTake;

        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damageToTake, bool shouldKnockBack)
    {
        TakeDamage(damageToTake);

        if(shouldKnockBack == true)
        {
            knockBackCounter = knockBackTime;
        }
    }
}