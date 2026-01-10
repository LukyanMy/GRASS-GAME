using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
  public float sensX;
  public float sensY;

  public Transform orientation;

  float xRotation;
  float yRotation;

  private void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void Update()
  {
    Vector2 mouseDelta = Mouse.current.delta.ReadValue();

    float mouseX = mouseDelta.x * Time.deltaTime * sensX;
    float mouseY = mouseDelta.y * Time.deltaTime * sensY;

    yRotation += mouseX;
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -80f, 90f);

    transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    orientation.rotation = Quaternion.Euler(0, yRotation, 0);
  }
}
