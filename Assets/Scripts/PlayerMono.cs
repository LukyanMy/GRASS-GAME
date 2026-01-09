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
    public float speedMultiplier = 0.5f;

    public float maxSpeed = 5f;

    public float idleDrag = 8f;

    public Vector3 inputDirection = Vector3.zero;

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
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    dir += Vector3.up;
        //}

        inputDirection = dir.normalized;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);
        float currentMax = maxSpeed * speedMultiplier;

        if (inputDirection != Vector3.zero)
        {
            Vector3 accel = inputDirection * speed * speedMultiplier * 1.3f;
            rb.AddForce(accel, ForceMode.Acceleration);
        }
        else
        {
            float factor = Mathf.Clamp01(idleDrag * Time.fixedDeltaTime);
            Vector3 newHorizontal = Vector3.Lerp(horizontal, Vector3.zero, factor);
            rb.linearVelocity = new Vector3(newHorizontal.x, vel.y, newHorizontal.z);

            // Update 'vel' and 'horizontal' variables to reflect changed velocity for subsequent clamping.
            vel = rb.linearVelocity;
            horizontal = new Vector3(vel.x, 0f, vel.z);
        }

        if (horizontal.magnitude > currentMax)
        {
            Vector3 clampedHorizontal = horizontal.normalized * currentMax;
            rb.linearVelocity = new Vector3(clampedHorizontal.x, vel.y, clampedHorizontal.z);
        }
    }
}