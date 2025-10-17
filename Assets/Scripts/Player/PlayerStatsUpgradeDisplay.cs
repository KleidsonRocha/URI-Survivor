using TMPro;
using UnityEngine;

public class PlayerStatsUpgradeDisplay : MonoBehaviour
{
    public GameObject upgradeButton;

    public void updateDisplay(int cost) 
    {
        if(cost <= CoinController.instance.currentCoins)
        {
            upgradeButton.SetActive(true);
        } else
        {
            upgradeButton.SetActive(false);
        }
    }
    

}
