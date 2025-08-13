using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configuracoes importantes para evitar empurrao permanente
        rb.linearDamping = 5f; // Adiciona resistencia natural
        rb.angularDamping = 5f;  // Adiciona resistencia natural
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate() // Use FixedUpdate com Rigidbody2D
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Movimento usando Rigidbody2D
        rb.linearVelocity = moveInput * moveSpeed;

        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}