using System;
using UnityEngine;

public class CloseAtackWeapon : Weapon
{
    public EnemyDamager damager;

    private float attackCounter;


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


            }
            Instantiate(damager, damager.transform.position, damager.transform.rotation, transform).gameObject.SetActive(true);

            for (int i = 1; i < stats[weaponLevel].amount; i++)
            {
                float rot = (360f / stats[weaponLevel].amount) * i;

                Instantiate(damager, damager.transform.position, Quaternion.Euler(0f, 0f,damager.transform.rotation.eulerAngles.z + rot), transform).gameObject.SetActive(true);
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