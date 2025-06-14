using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirControl : MonoBehaviour
{
    [Header("Air Control Settings")]
    public float rollSpeed = 100f;              // Degrees per second
    public float groundCheckDistance = 1.0f;
    public LayerMask groundLayer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!IsGrounded())
        {
            float rotationInput = 0f;

            if (Input.GetKey(KeyCode.Q))
                rotationInput = 1f;
            else if (Input.GetKey(KeyCode.E))
                rotationInput = -1f;

            if (rotationInput != 0f)
            {
                // Set angular velocity for continuous controlled roll
                rb.angularVelocity = transform.forward * rotationInput * rollSpeed * Mathf.Deg2Rad;
            }
            else
            {
                // Stop spinning when no key is pressed
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
