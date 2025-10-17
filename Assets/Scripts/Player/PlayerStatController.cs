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

    public int moveSpeedLevel, heathLevel, pickupEangeLevel, maxWeaponLevel;

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