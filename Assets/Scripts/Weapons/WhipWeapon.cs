using UnityEngine;

public class WhipWeapon : Weapon
{
    // Prefab que representa a �rea de colis�o/visual do chicote.
    // DEVE SER O COMPONENTE EnemyDamager DO SEU GameObject WhipDamagerTemplate na cena.
    public EnemyDamager damagerTemplate; // <-- ALTERADO AQUI

    // Um ponto de origem no jogador de onde o chicote visualmente parte (ex: a m�o do jogador).
    public Transform attackOriginPoint;

    private float attackCounter;
    private float lastPlayerFacingDirection = 1f; // Armazena a �ltima dire��o do jogador (1 para direita, -1 para esquerda)

    // Start � chamado antes da primeira atualiza��o de frame
    void Start()
    {
        // Inicializa o contador para o primeiro ataque.
        attackCounter = stats[weaponLevel].timeBetweenAttacks;
    }

    // Update � chamado uma vez por frame
    void Update()
    {
        // Verifica se as estat�sticas foram atualizadas (ex: subiu de n�vel)
        if (statsUpdated)
        {
            statsUpdated = false;
            // Reinicia o contador de ataque se as estat�sticas mudarem
            attackCounter = stats[weaponLevel].timeBetweenAttacks;
        }

        // Decrementa o contador de ataque
        attackCounter -= Time.deltaTime;

        // Se o contador chegar a zero ou menos, realiza um ataque
        if (attackCounter <= 0)
        {
            attackCounter = stats[weaponLevel].timeBetweenAttacks; // Reseta o contador
            PerformWhipAttack(); // Executa o ataque do chicote
        }
    }

    // M�todo para realizar o ataque do chicote
    void PerformWhipAttack()
    {
        float currentHorizontalInput = 0f;
        if (PlayerController.Instance != null)
        {
            currentHorizontalInput = PlayerController.Instance.GetMoveInput().x;
            if (currentHorizontalInput != 0)
            {
                lastPlayerFacingDirection = Mathf.Sign(currentHorizontalInput);
            }
        }

        float startAngleOffset = 0f;
        float arcSpan = 90f;

        if (lastPlayerFacingDirection > 0)
        {
            startAngleOffset = -arcSpan / 2f;
        }
        else
        {
            startAngleOffset = (180f - arcSpan / 2f) ;
        }

        int numWhipSegments = Mathf.Max(1, (int)stats[weaponLevel].amount);

        for (int i = 0; i < numWhipSegments; i++)
        {
            float angleIncrement = (numWhipSegments > 1) ? (arcSpan / (numWhipSegments - 1)) : 0f;
            float currentSegmentAngle = startAngleOffset + (angleIncrement * i);

            Quaternion rotation = Quaternion.Euler(0f, 0f, currentSegmentAngle);

            // INSTANCIANDO O GAMEOBJECT DO TEMPLATE DA CENA
            EnemyDamager newDamager = Instantiate(damagerTemplate.gameObject, attackOriginPoint.position, rotation).GetComponent<EnemyDamager>(); // <-- ALTERADO AQUI
            newDamager.gameObject.SetActive(true);

            newDamager.damageAmount = stats[weaponLevel].damage;
            newDamager.lifeTime = stats[weaponLevel].duration;

            newDamager.transform.localScale = new Vector3(stats[weaponLevel].range, newDamager.transform.localScale.y, newDamager.transform.localScale.z);
        }
    }
}