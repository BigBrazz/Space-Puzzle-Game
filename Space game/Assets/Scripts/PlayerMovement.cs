using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoverment : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float jumpForce = 6f;

    [Header("Crouch")]
    [Tooltip("Optional: assign the CameraHolder child to lower the camera while crouching.")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float standingCameraHeight = 0.8f;
    [SerializeField] private float crouchingCameraHeight = 0.4f;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private int groundContacts = 0;
    private bool isGrounded;
    private bool isCrouching;
    private bool jumpQueued;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        moveDirection = (horizontal * right + vertical * forward).normalized;

        isCrouching = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);
        if (cameraHolder != null)
        {
            Vector3 camPos = cameraHolder.localPosition;
            float targetHeight = isCrouching ? crouchingCameraHeight : standingCameraHeight;
            camPos.y = Mathf.Lerp(camPos.y, targetHeight, Time.deltaTime * 10f);
            cameraHolder.localPosition = camPos;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpQueued = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        groundContacts++;
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        groundContacts = Mathf.Max(0, groundContacts - 1);
        isGrounded = groundContacts > 0;
    }

    private void FixedUpdate()
    {
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);

        if (jumpQueued)
        {
            jumpQueued = false;
            Vector3 velocity = rb.velocity;
            velocity.y = jumpForce;
            rb.velocity = velocity;
        }
    }
}
