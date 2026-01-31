using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 20f;
    public float stickDeadzone = 0.2f;
    public float shootCooldown = 0.1f;
    public GameObject bulletPrefab;
    public Transform aimPoint;
    public EnemyShootPattern shootPattern;

    InputAction moveStickAction;
    InputAction moveKeyboardAction;
    InputAction aimStickAction;
    InputAction aimMouseAction;
    InputAction shootAction;

    Rigidbody rb;

    Camera mainCamera;

    Vector2 moveStickInput;
    Vector2 moveKeyboardInput;
    Vector2 aimStickInput;
    Vector2 mousePos;
    Vector2 oldMousePos;

    float timerShootCooldown;

    bool isUsingMouse;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        moveStickAction = InputSystem.actions.FindAction("MoveStick");
        moveKeyboardAction = InputSystem.actions.FindAction("MoveKeyboard");
        aimStickAction = InputSystem.actions.FindAction("AimStick");
        aimMouseAction = InputSystem.actions.FindAction("AimMouse");
        shootAction = InputSystem.actions.FindAction("Shoot");

        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        moveStickAction.Enable();
        moveKeyboardAction.Enable();
        aimStickAction.Enable();
        aimMouseAction.Enable();
        shootAction.Enable();
    }

    void OnDisable()
    {
        moveStickAction.Disable();
        moveKeyboardAction.Disable();
        aimStickAction.Disable();
        aimMouseAction.Disable();
        shootAction.Disable();
    }

    void Update()
    {
        moveStickInput = moveStickAction.ReadValue<Vector2>();
        moveKeyboardInput = moveKeyboardAction.ReadValue<Vector2>();
        aimStickInput = aimStickAction.ReadValue<Vector2>();
        mousePos = aimMouseAction.ReadValue<Vector2>();

        if (shootPattern != null && shootAction.IsPressed())
        {
            if (timerShootCooldown <= 0)
            {
                timerShootCooldown = shootCooldown;
                shootPattern.Fire(aimPoint, bulletPrefab, null);
            }
        }

        if (timerShootCooldown > 0) timerShootCooldown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        float deadzoneSquared = stickDeadzone * stickDeadzone;

        if (oldMousePos != mousePos) isUsingMouse = true;
        if (moveKeyboardInput.sqrMagnitude > deadzoneSquared) isUsingMouse = true;
        if (aimStickInput.sqrMagnitude > deadzoneSquared) isUsingMouse = false;
        if (moveStickInput.sqrMagnitude > deadzoneSquared) isUsingMouse = false;

        Vector2 moveInput = isUsingMouse ? moveKeyboardInput : moveStickInput;

        // Handle movement
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        // Handle aim
        if (!isUsingMouse)
        {
            // Aim using stick

            // Hide & lock the mouse
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            if (aimStickInput.sqrMagnitude > deadzoneSquared)
            {
                Vector3 dir = new Vector3(aimStickInput.x, 0f, aimStickInput.y);
                Quaternion target = Quaternion.LookRotation(dir);

                rb.MoveRotation(
                    Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime)
                );
            }
            else if (moveStickInput.sqrMagnitude > deadzoneSquared)
            {
                // Aim in the move direction
                Vector3 dir = new Vector3(moveStickInput.x, 0f, moveStickInput.y);
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
            Plane plane = new Plane(Vector3.up, transform.position);

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
    }
}