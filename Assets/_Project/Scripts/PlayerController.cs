using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 20f;
    public float stickDeadzone = 0.2f;

    [Header("Shooting")]
    public float shootCooldown = 0.1f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.12f;
    public float dashCooldown = 0.4f;

    [Header("Knockback")]
    public float knockbackSpeed = 30f;
    public float knockbackDuration = 0.08f;
    public float invicibilityDuration = 1f;
    public float blinkSpeed = 0.1f;

    [Header("Refs")]
    public GameObject bulletPrefab;
    public GameObject visuals;
    public Transform aimPoint;
    public EnemyShootPattern shootPattern;
    public string shardTag = "Shard";

    [Header("Audio")]
    public AudioSource hurtAudio;
    public AudioSource shootAudio;
    public AudioSource dashAudio;

    InputAction moveStickAction;
    InputAction moveKeyboardAction;
    InputAction aimStickAction;
    InputAction aimMouseAction;
    InputAction shootAction;
    InputAction dashAction;

    Rigidbody rb;
    Health health;

    Camera mainCamera;
    GameManager game;

    Vector2 moveStickInput;
    Vector2 moveKeyboardInput;
    Vector2 aimStickInput;
    Vector2 mousePos;
    Vector2 oldMousePos;
    Vector3 dashDirection;
    Vector3 knockbackDirection;

    float timerShootCooldown;
    float dashTimer;
    float dashCooldownTimer;
    float knockbackTimer;
    float invicibilityTimer;
    float blinkTimer;

    bool isUsingMouse;
    bool isDashing;
    bool isKnockbacking;
    bool isVisible = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        hurtAudio = GetComponent<AudioSource>();
        health.onDamage.AddListener(OnDamage);
        game = FindAnyObjectByType<GameManager>();

        moveStickAction = InputSystem.actions.FindAction("MoveStick");
        moveKeyboardAction = InputSystem.actions.FindAction("MoveKeyboard");
        aimStickAction = InputSystem.actions.FindAction("AimStick");
        aimMouseAction = InputSystem.actions.FindAction("AimMouse");
        shootAction = InputSystem.actions.FindAction("Shoot");
        dashAction = InputSystem.actions.FindAction("Dash");

        mainCamera = Camera.main;
    }

    private void OnDamage(GameObject source)
    {
        knockbackDirection = Vector3.Normalize(transform.position - source.transform.position);
        knockbackTimer = knockbackDuration;
        isKnockbacking = true;
        health.isInvicible = true;
        invicibilityTimer = invicibilityDuration;
        blinkTimer = blinkSpeed;

        CameraController.Instance.Shake();
        hurtAudio.Play();
    }

    void OnEnable()
    {
        moveStickAction.Enable();
        moveKeyboardAction.Enable();
        aimStickAction.Enable();
        aimMouseAction.Enable();
        shootAction.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        moveStickAction.Disable();
        moveKeyboardAction.Disable();
        aimStickAction.Disable();
        aimMouseAction.Disable();
        shootAction.Disable();
        dashAction.Disable();
    }

    void Update()
    {
        moveStickInput = moveStickAction.ReadValue<Vector2>();
        moveKeyboardInput = moveKeyboardAction.ReadValue<Vector2>();
        aimStickInput = aimStickAction.ReadValue<Vector2>();
        mousePos = aimMouseAction.ReadValue<Vector2>();

        if (game.IsPaused || game.IsOvered || !game.IsStarted) return;

        if (shootPattern != null && shootAction.IsPressed())
        {
            if (timerShootCooldown <= 0)
            {
                timerShootCooldown = shootCooldown;
                shootPattern.Fire(aimPoint, bulletPrefab, null);
                shootAudio.Play();
            }
        }

        if (timerShootCooldown > 0) timerShootCooldown -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (!isDashing && dashCooldownTimer <= 0f && dashAction.WasPressedThisFrame())
        {
            Vector2 moveInput = isUsingMouse ? moveKeyboardInput : moveStickInput;

            if (moveInput.sqrMagnitude > stickDeadzone * stickDeadzone)
            {
                health.isInvicible = true;
                isDashing = true;
                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;

                dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

                dashAudio.Play();
            }
        }


        if (invicibilityTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                blinkTimer = blinkSpeed;
                isVisible = !isVisible;
                visuals.SetActive(isVisible);
            }

            invicibilityTimer -= Time.deltaTime;
            if (invicibilityTimer <= 0)
            {
                health.isInvicible = false;
                isVisible = true;
                visuals.SetActive(isVisible);
            }
        }
    }

    void FixedUpdate()
    {
        if (game.IsPaused || game.IsOvered) return;

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                health.isInvicible = false;
                isDashing = false;
            }

            // Skip normal movement while dashing
            return;
        }

        if (isKnockbacking)
        {
            rb.linearVelocity = knockbackDirection * knockbackSpeed;
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockbacking = false;
            }

            // Skip normal movement while knockbacking
            return;
        }


        float deadzoneSquared = stickDeadzone * stickDeadzone;

        if (oldMousePos != mousePos) isUsingMouse = true;
        if (moveKeyboardInput.sqrMagnitude > deadzoneSquared) isUsingMouse = true;
        if (aimStickInput.sqrMagnitude > deadzoneSquared) isUsingMouse = false;
        if (moveStickInput.sqrMagnitude > deadzoneSquared) isUsingMouse = false;

        // Handle aim
        if (!isUsingMouse)
        {
            // Aim using stick

            // Hide & lock the mouse
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (aimStickInput.sqrMagnitude > deadzoneSquared)
            {
                Vector3 dir = new(aimStickInput.x, 0f, aimStickInput.y);
                Quaternion target = Quaternion.LookRotation(dir);

                rb.MoveRotation(
                    Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime)
                );
            }
            else if (moveStickInput.sqrMagnitude > deadzoneSquared)
            {
                // Aim in the move direction
                Vector3 dir = new(moveStickInput.x, 0f, moveStickInput.y);
                Quaternion target = Quaternion.LookRotation(dir);

                rb.MoveRotation(
                    Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime)
                );
            }
        }
        else
        {
            // Aim using mouse

            // Show the mouse
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            Plane plane = new(Vector3.up, transform.position);

            if (!plane.Raycast(ray, out float dist))
                return;

            Vector3 point = ray.GetPoint(dist);
            Vector3 dir = point - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude < 0.001f)
                return;

            Quaternion target = Quaternion.LookRotation(dir);
            Quaternion rotation = Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(rotation);
            oldMousePos = mousePos;
        }

        if (!game.IsStarted) return;

        Vector2 moveInput = isUsingMouse ? moveKeyboardInput : moveStickInput;

        // Handle movement
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(shardTag))
        {
            game.ShardIncrement();
            Debug.Log("Adding shards");
            Destroy(other.gameObject);
        }
    }
}