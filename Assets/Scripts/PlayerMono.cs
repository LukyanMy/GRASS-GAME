using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMono : MonoBehaviour
{
    private Keyboard keyboard = Keyboard.current;
    Rigidbody rb;
    public int health = 100;
    public float armor = 1;
    const float speed = 3f; 
    public int speedMultiplier = 1;

    public float maxSpeed = 5f;

    Vector3 inputDirection = Vector3.zero;

    void Start()
    {
        Debug.Log("Player initialized with health: " + health + " and armor: " + armor * 100);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }

        inputDirection = dir.normalized;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (inputDirection != Vector3.zero)
        {
            // Compute acceleration vector (m/s^2)
            Vector3 accel = inputDirection * speed * speedMultiplier;
            rb.AddForce(accel, ForceMode.Acceleration);
        }

        // Clamp horizontal velocity to maxSpeed * speedMultiplier
        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);
        float currentMax = maxSpeed * speedMultiplier;
        if (horizontal.magnitude > currentMax)
        {
            Vector3 clampedHorizontal = horizontal.normalized * currentMax;
            rb.linearVelocity = new Vector3(clampedHorizontal.x, vel.y, clampedHorizontal.z);
        }
    }
}