using Unity.Profiling;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed;


    // Start é chamado antes do primeiro frame
    void Start()
    {

    }

    // Update é chamado uma vez por frame
    void Update()
    {
        Vector3 moveInput = new Vector3(0f, 0f, 0f);
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        //Debug.Log(moveInput);

        transform.position += moveInput * moveSpeed * Time.deltaTime;
    }
}