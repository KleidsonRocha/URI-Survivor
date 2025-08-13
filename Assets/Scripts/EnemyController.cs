using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;
    public float stopDistance = 0f; // Distância para parar
    private Transform target;

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        // Só se move se estiver longe o suficiente
        if (distanceToTarget > stopDistance)
        {
            theRB.linearVelocity = (target.position - transform.position).normalized * moveSpeed;
        }
        else
        {
            theRB.linearVelocity = Vector2.zero; // Para o movimento
        }
    }
}