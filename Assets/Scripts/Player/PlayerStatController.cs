using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    public static PlayerStatController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<PlayerStatValue> movespeed, heath, pickupRange, maxWeapons;
    public int moveSpeedLevelCount, healthLevelCount, pickupRangeLevelCount;

    public int moveSpeedLevel, heathLevel, pickupRangeLevel, maxWeaponLevel;

    void Start()
    {
        for (int i = movespeed.Count - 1; i < moveSpeedLevelCount; i++)
        {
            movespeed.Add(new PlayerStatValue(movespeed[i].cost + movespeed[1].cost, movespeed[i].value + (movespeed[1].value - movespeed[0].value)));
        }
        for (int i = heath.Count - 1; i < healthLevelCount; i++)
        {
            heath.Add(new PlayerStatValue(heath[i].cost + heath[1].cost, heath[i].value + (heath[1].value - heath[0].value)));
        }
        for (int i = pickupRange.Count - 1; i < pickupRangeLevelCount; i++)
        {
            pickupRange.Add(new PlayerStatValue(pickupRange[i].cost + pickupRange[1].cost, pickupRange[i].value + (pickupRange[1].value - pickupRange[0].value)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.instance.levelUpPanel.activeSelf == true)
        {
            UpdateDisplay();
        }
    }

    public void UpdateDisplay()
    {

        UIController.instance.moveSpeedUpgradeDisplay.updateDisplay(movespeed[moveSpeedLevel +1].cost);
        UIController.instance.healthUpgradeDisplay.updateDisplay(heath[heathLevel + 1].cost);
        UIController.instance.pickupRangeUpgradeDisplay.updateDisplay(pickupRange[pickupRangeLevel + 1].cost);
        UIController.instance.maxWeaponsUpgradeDisplay.updateDisplay(maxWeapons[maxWeaponLevel +1].cost);

    }

    public void PurchaseMoveSpeed()
    {
        moveSpeedLevel++;
        CoinController.instance.SpendCoins(movespeed[moveSpeedLevel].cost);
        UpdateDisplay();

        PlayerController.Instance.moveSpeed = movespeed[moveSpeedLevel].value;
    }
    public void PurchaseHealth()
    {
        heathLevel++;
        CoinController.instance.SpendCoins(heath[heathLevel].cost);
        UpdateDisplay();

        PlayerHealthController.instance.maxHealth = heath[heathLevel].value;
        PlayerHealthController.instance.currentHealth = heath[heathLevel].value - heath[heathLevel -1].value;

    }

    public void PurchaseRange()
    {
        pickupRangeLevel++;
        CoinController.instance.SpendCoins(pickupRange[pickupRangeLevel].cost);
        UpdateDisplay();

        PlayerController.Instance.pickupRange = pickupRange[pickupRangeLevel].value;
    }
    public void PurchaseMaxWeapons()
    {
        maxWeaponLevel++;
        CoinController.instance.SpendCoins(maxWeapons[maxWeaponLevel].cost);
        UpdateDisplay();

        PlayerController.Instance.maxWeapons = Mathf.RoundToInt(maxWeapons[maxWeaponLevel].value);
    }
}

[System.Serializable]

public class PlayerStatValue
{
    public int cost;
    public float value;

    public PlayerStatValue(int newCost, float newValue)
    {
        cost = newCost;
        value = newValue;

    }

}