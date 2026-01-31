using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 20f;
    public float stickDeadzone = 0.2f;

    InputAction moveAction;
    InputAction aimStickAction;
    InputAction aimMouseAction;

    Rigidbody rb;

    Camera mainCamera;

    Vector2 moveInput;
    Vector2 stickInput;
    Vector2 mousePos;
    Vector2 oldMousePos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");
        aimStickAction = InputSystem.actions.FindAction("AimStick");
        aimMouseAction = InputSystem.actions.FindAction("AimMouse");

        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        moveAction.Enable();
        aimStickAction.Enable();
        aimMouseAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        aimStickAction.Disable();
        aimMouseAction.Disable();
    }

    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        stickInput = aimStickAction.ReadValue<Vector2>();
        mousePos = aimMouseAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Handle movement
        Vector3 velocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

        float deadzoneSquared = stickDeadzone * stickDeadzone;

        // Handle aim
        if (stickInput.sqrMagnitude > deadzoneSquared)
        {
            // Aim using stick

            // Hide & lock the mouse
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Vector3 dir = new Vector3(stickInput.x, 0f, stickInput.y);
            Quaternion target = Quaternion.LookRotation(dir);

            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime)
            );
        }
        else if (oldMousePos != mousePos)
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
        else if (moveInput.sqrMagnitude > deadzoneSquared)
        {
            // Aim in the move direction
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);
            Quaternion target = Quaternion.LookRotation(dir);

            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, target, rotationSpeed * Time.fixedDeltaTime)
            );
        }
    }
}