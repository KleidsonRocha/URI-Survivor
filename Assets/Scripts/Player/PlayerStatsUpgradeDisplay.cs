using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUpgradeDisplay : MonoBehaviour
{
    public GameObject upgradeButtonGameObject;
    private Button upgradeButtonComponent; 
    private Image buttonImage;

    public Color enabledColor = Color.white; 
    public Color disabledColor = new Color(1f, 0.75f, 0.8f); 

    void Awake()
    {
       
        upgradeButtonComponent = upgradeButtonGameObject.GetComponent<Button>();
        buttonImage = upgradeButtonGameObject.GetComponent<Image>();

        if (upgradeButtonComponent == null)
        {
            Debug.LogError("O GameObject do botão de upgrade não possui um componente Button!", this);
        }
        if (buttonImage == null)
        {
            Debug.LogError("O GameObject do botão de upgrade não possui um componente Image!", this);
        }
    }

    public void updateDisplay(int cost)
    {
       
        upgradeButtonGameObject.SetActive(true);

        if (CoinController.instance.currentCoins >= cost)
        {
            
            if (buttonImage != null)
            {
                buttonImage.color = enabledColor;
            }
            
            if (upgradeButtonComponent != null)
            {
                upgradeButtonComponent.interactable = true;
            }
        }
        else
        {
           
            if (buttonImage != null)
            {
                buttonImage.color = disabledColor;
            }
           
            if (upgradeButtonComponent != null)
            {
                buttonImage.color = disabledColor; 
                upgradeButtonComponent.interactable = false;
            }
        }
    }
}