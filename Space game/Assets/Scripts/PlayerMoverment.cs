using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverment : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f; 
    private Rigidbody rb; 
    private Vector3 moveDirection;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical"); 

        Vector3 forward = transform.forward; 
        Vector3 right = transform.right; 

        moveDirection = (horizontal * right + vertical * forward).normalized;
    } 

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
