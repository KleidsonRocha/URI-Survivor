using UnityEngine;

public class SpinWeapon : Weapon
{
    public float rotateSpeed;

    public Transform holder, fireballToSawn;

    public float timeBetweenSpawn;
    private float spawnCounter;

    public EnemyDamager damager;

    void Start()
    {
        SetStats();


    }

    void Update()
    {
        holder.rotation = Quaternion.Euler(0f, 0f, holder.rotation.eulerAngles.z + (rotateSpeed * Time.deltaTime * stats[weaponLevel].speed));

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = timeBetweenSpawn;

            Instantiate(fireballToSawn, fireballToSawn.position, fireballToSawn.rotation, holder).gameObject.SetActive(true);
        }

        if(statsUpdated == true)
        {
            statsUpdated = false;

            SetStats();
        }
    }

    public void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;

        transform.localScale = Vector3.one * stats[weaponLevel].range;

        timeBetweenSpawn = stats[weaponLevel].timeBetweenAttacks;

        damager.lifeTime = stats[weaponLevel].duration;

        spawnCounter = 0f;
    }
}
