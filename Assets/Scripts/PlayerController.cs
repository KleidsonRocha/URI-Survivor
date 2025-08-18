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

        // Configurações importantes para evitar empurrão permanente
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Movimento usando Rigidbody2D
        rb.linearVelocity = moveInput * moveSpeed;

        // Controle da animação
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isMoving", true);

            // Espelhar sprite baseado na direção horizontal
            if (moveInput.x > 0) // Movendo para direita
            {
                spriteRenderer.flipX = false; // Sprite normal (olhando para direita)
            }
            else if (moveInput.x < 0) // Movendo para esquerda
            {
                spriteRenderer.flipX = true; // Sprite espelhado (olhando para esquerda)
            }
            // Se moveInput.x == 0 (só movimento vertical), mantém a direção atual
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }
}