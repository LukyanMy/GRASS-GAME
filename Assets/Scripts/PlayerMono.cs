using System;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
    void Start()
    {
        int health = 100;
        int armor = 1;
        const int speed = 10;
        int speedMultiplier = 1;
        Debug.Log("Player initialized with health: " + health + " and armor: " + armor * 100);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) Destroy(this.gameObject);
    }
}