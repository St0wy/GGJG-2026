using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
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
    public AudioSource hurtAudio;

    //private CharacterController cc;
    Rigidbody rb;
    private float t0;
    private float nextShotTime;
    private float yVel;

    EnemyManager manager;
    GameManager gameManager;
    Health health;

    Rigidbody rigid;
    RigidbodyConstraints constraints;

    bool oldStart;

    private void Awake()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
        
        rigid = GetComponent<Rigidbody>();
        constraints = rigid.constraints;

        //cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        t0 = Time.time;
        nextShotTime = Time.time + fireStartDelay;
        gameManager = FindAnyObjectByType<GameManager>();

        health = GetComponent<Health>();
        health.onDamage.AddListener(OnDamage);
        health.onDeath.AddListener(OnDeath);
    }

    private void OnDeath(GameObject source)
    {
        foreach (var c in GetComponents<Collider>())
            c.enabled = false;
        rb.isKinematic = true;

        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        if (source.TryGetComponent(out SimpleProjectile projectile))
        {
            Destroy(projectile.gameObject);
        }

        Destroy(gameObject, 0.3f);
    }

    private void OnDamage(GameObject arg0)
    {
        hurtAudio.Play();
    }

    private void Start()
    {
        manager = EnemyManager.Instance;
        if (manager) manager.Register(this);
    }

    private void Update()
    {
        if (health.isDead) return;
        if (gameManager.IsPaused) return;
        if (!gameManager.IsStarted) return;
        if (gameManager.IsOvered) return;
        if (!oldStart && gameManager.IsStarted)
        {
            nextShotTime = Time.time + fireStartDelay;
            t0 = Time.time;
        }

        HandleRotation();
        HandleMovement();
        HandleShooting();

        oldStart = gameManager.IsStarted;
    }

    private void OnDestroy()
    {
        if (manager) manager.Unregister(this);
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

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
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