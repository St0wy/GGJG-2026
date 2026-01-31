using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Patterns")]
    public EnemyMovePattern movePattern;
    public EnemyShootPattern shootPattern;

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;     // tirs/sec
    public float fireStartDelay = 0.4f;

    [Header("Rotation")]
    public bool rotateTowardTarget = true;
    public float rotationSpeed = 8f;

    [Header("Movement")]
    public bool stickToGround = true;
    public float gravity = -20f;

    private CharacterController cc;
    private float t0;
    private float nextShotTime;
    private float yVel;

    EnemyManager manager;

    Rigidbody rigid;
    RigidbodyConstraints constraints;

    private void Awake()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
        
        rigid = GetComponent<Rigidbody>();
        constraints = rigid.constraints;

        cc = GetComponent<CharacterController>();
        t0 = Time.time;
        nextShotTime = Time.time + fireStartDelay;
    }

    private void Start()
    {
        manager = EnemyManager.Instance;
        if (manager) manager.Register(this);
    }

    private void Update()
    {
        if (manager.IsPaused) return;

        HandleRotation();
        HandleMovement();
        HandleShooting();
    }

    private void OnDestroy()
    {
        if (manager) manager.Unregister(this);
    }

    public void SetPaused(bool paused)
    {
        if (paused)
        {
            // stop net
            yVel = 0f;
            rigid.constraints = rigid.constraints | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rigid.constraints = constraints;
            // resync timers pour éviter un tir instantané à la reprise
            nextShotTime = Time.time + fireStartDelay;
            t0 = Time.time; // optionnel: remet le pattern à zéro
        }
    }

    private void HandleRotation()
    {
        if (!rotateTowardTarget || target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        // Si trop proche / direction nulle => ne pas recalculer une rotation
        if (toTarget.sqrMagnitude < 0.05f) return;

        Quaternion desired = Quaternion.LookRotation(toTarget.normalized, Vector3.up);

        // Rotation uniquement sur Y (empêche pitch/roll)
        Vector3 e = desired.eulerAngles;
        desired = Quaternion.Euler(0f, e.y, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, desired, rotationSpeed * Time.deltaTime);
    }

    private void HandleMovement()
    {
        Vector3 velocity = Vector3.zero;

        if (movePattern != null)
        {
            float t = Time.time - t0;
            velocity = movePattern.GetVelocity(t, transform, target);
        }

        // gravité + sol
        if (stickToGround)
        {
            if (cc.isGrounded && yVel < 0f) yVel = -1f;
            yVel += gravity * Time.deltaTime;
            velocity.y = yVel;
        }

        cc.Move(velocity * Time.deltaTime);
    }

    private void HandleShooting()
    {
        if (shootPattern == null || projectilePrefab == null || firePoint == null) return;
        if (fireRate <= 0f) return;

        if (Time.time >= nextShotTime)
        {
            shootPattern.Fire(firePoint, projectilePrefab, target);
            nextShotTime = Time.time + (1f / fireRate);
        }
    }
}