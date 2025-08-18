using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private InputActionReference moveActionToUse;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Configurações importantes para evitar empurrão permanente
        rb.linearDamping = 5f;
        rb.angularDamping = 5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Lê o input do novo Input System
        if (moveActionToUse != null)
        {
            moveInput = moveActionToUse.action.ReadValue<Vector2>();
        }
    }

    void FixedUpdate()
    {
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
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    void OnEnable()
    {
        if (moveActionToUse != null)
            moveActionToUse.action.Enable();
    }

    void OnDisable()
    {
        if (moveActionToUse != null)
            moveActionToUse.action.Disable();
    }
}