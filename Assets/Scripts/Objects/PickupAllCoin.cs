using UnityEngine;


public class PickupAllCoin : MonoBehaviour
{
    public float lifetime = 15f;


    private bool hasBeenCollected = false;

    void Start()
    {

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenCollected)
        {
            ActivateMagnetismOnAllItems(); 
            hasBeenCollected = true;
            SFXManager.instance.PlaySFXPitched(2);

            Destroy(gameObject);
        }
    }

    private void ActivateMagnetismOnAllItems()
    {
        ExpPickup[] expOrbs = FindObjectsByType<ExpPickup>(FindObjectsSortMode.None);
        foreach (ExpPickup orb in expOrbs)
        {
            orb.StartMagnetizing(); 
        }
      
  
        CoinPickup[] regularCoins = FindObjectsByType<CoinPickup>(FindObjectsSortMode.None);
        foreach (CoinPickup coin in regularCoins)
        {
            coin.StartMagnetizing(); 
        }
     
    }
}