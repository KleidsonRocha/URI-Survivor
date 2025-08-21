using UnityEngine;
using TMPro;
using UnityEngine.Windows;

public class DamageNumber : MonoBehaviour 
{
    public TMP_Text damageText;

    public float lifetime;
    private float lifeCounter;

    public float floatSpeed = 0.5f;

    void Update()
    {
        if (lifeCounter > 0)
        {
            lifeCounter -= Time.deltaTime;

            if(lifeCounter <= 0)
            {
                //Destroy(gameObject);

                DamageNumberController.instance.PlaceInPool(this);
            }
        }

        /*
        if (UnityEngine.Input.GetKeyDown(KeyCode.U))
        {
           Setup(45);
        }
        */

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

    public void Setup(int damageDisplay)
    {
        lifeCounter = lifetime;

        damageText.text = damageDisplay.ToString();
    }
}
