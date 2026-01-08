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
    const float speed = 10f;
    public int speedMultiplier = 1;


    void Start()
    {
        Debug.Log("Player initialized with health: " + health + " and armor: " + armor * 100);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetKeyDown("space") ? 0: 1);
        transform.position = transform.position + playerInput.normalized * speed * Time.deltaTime;
    }

}