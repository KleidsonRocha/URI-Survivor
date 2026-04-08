using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount = 10f;
    public float lifetime = 15f;

    private bool hasBeenCollected = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || hasBeenCollected)
        {
            return;
        }

        if (PlayerHealthController.instance == null || !PlayerHealthController.instance.gameObject.activeInHierarchy)
        {
            return;
        }

        if (PlayerHealthController.instance.currentHealth >= PlayerHealthController.instance.maxHealth)
        {
            return;
        }

        hasBeenCollected = true;
        PlayerHealthController.instance.Heal(healAmount);
        SFXManager.instance.PlaySFXPitched(2);

        Destroy(gameObject);
    }
}
