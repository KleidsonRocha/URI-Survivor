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

}