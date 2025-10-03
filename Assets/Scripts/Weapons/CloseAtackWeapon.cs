using System;
using UnityEngine;
// Adicione esta linha se PlayerController estiver em um namespace diferente, o que geralmente não é o caso para scripts simples.
// using YourGameNamespace; 

public class CloseAtackWeapon : Weapon
{
    public EnemyDamager damager;

    private float attackCounter, direction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (statsUpdated == true)
        {
            statsUpdated = false;

            SetStats();
        }

        attackCounter -= Time.deltaTime;

        if (attackCounter <= 0)
        {
            attackCounter = stats[weaponLevel].timeBetweenAttacks;

            float horizontalInput = 0f;
            if (PlayerController.Instance != null) 
            {

                horizontalInput = PlayerController.Instance.GetMoveInput().x;
            }



            if (horizontalInput != 0)
            {
                if (horizontalInput > 0)
                {
                    damager.transform.rotation = Quaternion.identity;
                }
                else
                {
                    damager.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                }


                Instantiate(damager, damager.transform.position, damager.transform.rotation, transform).gameObject.SetActive(true);
            }
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;

        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;

        attackCounter = 0f;
    }
}