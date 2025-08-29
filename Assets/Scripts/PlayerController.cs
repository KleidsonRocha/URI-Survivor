using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Weapon> unassignedWeapons, assignedWeapons;

    public float moveSpeed;
    public Animator anim;
    public float pickupRange = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private InputActionReference moveActionToUse;
    private Vector2 moveInput;
    private bool wasMoving = false; // Para detectar quando começou/parou de se mover

    void Start()
    {
        AddWeapon(Random.Range(0, unassignedWeapons.Count));   


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

        // Detecta se está se movendo
        bool isCurrentlyMoving = moveInput != Vector2.zero;

        // Controle da animação
        if (isCurrentlyMoving)
        {
            // Se não estava se movendo antes, começou a se mover agora
            if (!wasMoving)
            {
                anim.SetBool("isMoving", true);
            }

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
            // Parou de se mover
            if (wasMoving)
            {
                anim.SetBool("isMoving", false);
            }
        }

        // Atualiza o estado anterior
        wasMoving = isCurrentlyMoving;
    }

    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber < unassignedWeapons.Count)
        {
            assignedWeapons.Add(unassignedWeapons[weaponNumber]);

            unassignedWeapons[weaponNumber].gameObject.SetActive(true);
            unassignedWeapons.RemoveAt(weaponNumber);
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