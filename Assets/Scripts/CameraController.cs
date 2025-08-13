using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }


    void LateUpdate()
    {
        if (target != null)
        {
            // Segue o player mantendo a posi��o Z da c�mera
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}