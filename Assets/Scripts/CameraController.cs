using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        // Encontra o player automaticamente
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado!");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Segue o player mantendo a posição Z da câmera
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}