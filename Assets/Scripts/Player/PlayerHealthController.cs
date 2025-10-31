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


        HealthSlider.maxValue = maxHealth;
        HealthSlider.value = currentHealth;
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
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            //destiva o player
            gameObject.SetActive(false);
            UIController.instance.levelEndScreen.SetActive(true);
        }

        HealthSlider.value = currentHealth;
    }
}
