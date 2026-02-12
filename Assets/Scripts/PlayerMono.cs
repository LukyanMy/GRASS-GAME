using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMono : MonoBehaviour
{
    private Keyboard keyboard = Keyboard.current;
    private Rigidbody rb;
    public int health = 100;
    public float armor = 1;
    private const float speed = 3f;

    // jump
    public float jumpConst;
    public float jumpStrength;

    // speed
    public float speedMultiplier = 0.5f;
    public float maxSpeed = 5f;
    public float idleDrag = 8f;

    public Vector3 inputDirection = Vector3.zero;

    public float maxJumpHoldTime = 0.35f;
    private float jumpHoldTimer = 0f;
    private bool groundCollided = false;

    // angles
    [SerializeField] private Camera cam;

    [SerializeField] private Collider groundRig;

    // ground check
    private readonly HashSet<Collider> groundContactColliders = new HashSet<Collider>();

    void Start()
    {
        Debug.Log("Player initialized with health: " + health + " and armor: " + armor * 100);
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody component missing on PlayerMono.");

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
        if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
        if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
        if (Input.GetKey(KeyCode.Space)) dir += Vector3.up;

        inputDirection = dir.normalized;
    }

    void FixedUpdate()
    {
        // rotation
        if (cam == null || rb == null)
            return;

        Vector3 rot = cam.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        Quaternion yaw = Quaternion.Euler(0f, rot.y, 0f);

        // Use local velocity variables (do not reference instance fields in initializers)
        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);
        float currentMax = maxSpeed * speedMultiplier;

        bool isHoldingJump = inputDirection.y > 0f;

        if (isHoldingJump)
        {
            jumpHoldTimer += Time.fixedDeltaTime;
        }
        else
        {
            jumpHoldTimer = 0f;
        }

        // jump checker
        if (isHoldingJump && jumpHoldTimer <= maxJumpHoldTime && groundCollided)
        {
            if (jumpConst >= Time.fixedDeltaTime)
            {
                Vector3 accel = Vector3.up * speed * speedMultiplier * jumpStrength;
                rb.AddForce(accel, ForceMode.Acceleration);
            }
            else
            {
                Vector3 accel = Vector3.up * jumpStrength;
                rb.AddForce(accel, ForceMode.Acceleration);
            }
        }
        else if (isHoldingJump && jumpHoldTimer > maxJumpHoldTime)
        {
            vel = rb.linearVelocity;
            horizontal = new Vector3(vel.x, 0f, vel.z);
        }

        // Horizontal (movement)
        Vector3 horizontalInput = new Vector3(inputDirection.x, 0f, inputDirection.z);
        if (horizontalInput != Vector3.zero)
        {
            Vector3 move = yaw * horizontalInput.normalized;
            Vector3 accel = move * (speed * speedMultiplier * 1.3f);
            rb.AddForce(new Vector3(accel.x, 0f, accel.z), ForceMode.Acceleration);
        }
        else
        {
            float factor = Mathf.Clamp01(idleDrag * Time.fixedDeltaTime);
            Vector3 newHorizontal = Vector3.Lerp(horizontal, Vector3.zero, factor);
            rb.linearVelocity = new Vector3(newHorizontal.x, vel.y, newHorizontal.z);

            // Update local vel and horizontal after changing velocity
            vel = rb.linearVelocity;
            horizontal = new Vector3(vel.x, 0f, vel.z);
        }

        // horizontal speed clamp
        if (horizontal.magnitude > currentMax)
        {
            Vector3 clampedHorizontal = horizontal.normalized * currentMax;
            rb.linearVelocity = new Vector3(clampedHorizontal.x, vel.y, clampedHorizontal.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts == null || collision.contacts.Length == 0)
            return;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f && (groundRig == null || collision.collider != groundRig))
            {
                groundContactColliders.Add(collision.collider);
                groundCollided = groundContactColliders.Count > 0;
                break;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts == null || collision.contacts.Length == 0)
        {
            if (groundContactColliders.Remove(collision.collider))
                groundCollided = groundContactColliders.Count > 0;
            return;
        }

        bool hasGroundContact = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
            {
                hasGroundContact = true;
                break;
            }
        }

        if (hasGroundContact)
        {
            groundContactColliders.Add(collision.collider);
            groundCollided = true;
        }
        else
        {
            if (groundContactColliders.Remove(collision.collider))
                groundCollided = groundContactColliders.Count > 0;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (groundContactColliders.Remove(collision.collider))
            groundCollided = groundContactColliders.Count > 0;
    }
}