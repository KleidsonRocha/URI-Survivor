using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinAmount;

    private bool movingToPlayer = false; 
    public float moveSpeed = 7f; 
    public float magnetizeSpeedMultiplier = 2f;



    private Transform playerTransform;

    void Start()
    {
       
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
            
        }
    }

    void Update()
    {
        if (movingToPlayer == true && playerTransform != null)
        {
          
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, (moveSpeed * magnetizeSpeedMultiplier) * Time.deltaTime);
        }
        
    }

    
    public void StartMagnetizing()
    {
        movingToPlayer = true;
      
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CoinController.instance != null)
            {
                CoinController.instance.AddCoins(coinAmount);
            }
            Destroy(gameObject);
        }
    }
}