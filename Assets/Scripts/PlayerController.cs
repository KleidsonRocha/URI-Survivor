using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Adicione esta linha

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Pega o SpriteRenderer do filho

        // Configura��es importantes para evitar empurr�o permanente
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Movimento usando Rigidbody2D
        rb.linearVelocity = moveInput * moveSpeed;

        // Controle da anima��o
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isMoving", true);

            // Espelhar sprite baseado na dire��o horizontal
            if (moveInput.x > 0) // Movendo para direita
            {
                spriteRenderer.flipX = false; // Sprite normal (olhando para direita)
            }
            else if (moveInput.x < 0) // Movendo para esquerda
            {
                spriteRenderer.flipX = true; // Sprite espelhado (olhando para esquerda)
            }
            // Se moveInput.x == 0 (s� movimento vertical), mant�m a dire��o atual
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}