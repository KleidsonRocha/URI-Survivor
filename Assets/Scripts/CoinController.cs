using UnityEngine;

public class CoinController : MonoBehaviour
{
    public static CoinController instance;
    void Start()
    {
        instance = this;
    }

    public int currentCoins;

    public CoinPickup coin;

    public void AddCoins(int coinsToAdd)
    {
        currentCoins += coinsToAdd;

        UIController.instance.updateCoins();
    }

    public void DropCooin(Vector3 position, int value)
    {
        CoinPickup newCoin = Instantiate(coin, position + new Vector3(.2f, .1f, 0f), Quaternion.identity);

        newCoin.coinAmount = value;
        newCoin.gameObject.SetActive(true);

    }
}
