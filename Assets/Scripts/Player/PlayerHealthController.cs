using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentHealth, maxHealth;
    public Slider HealthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        RefreshHealthUI();
    }


    void Update()
    {


        //testar o dano
        if (Input.GetKeyDown(KeyCode.T))
        {
           TakeDamage(10f);
        }
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth = Mathf.Max(currentHealth - damageToTake, 0f);
        RefreshHealthUI();

        if (currentHealth <= 0)
        {
            //destiva o player
            gameObject.SetActive(false);
            UIController.instance.levelEndScreen.SetActive(true);
            SFXManager.instance.PlaySFX(3);
        }
    }

    public void Heal(float amountToHeal)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amountToHeal, maxHealth);
        RefreshHealthUI();
    }

    public void RefreshHealthUI()
    {
        if (HealthSlider == null)
        {
            return;
        }

        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = currentHealth;
    }
}
