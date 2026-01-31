using UnityEngine;

public class SecondProjectile : MonoBehaviour
{
    public int bounceCount = 5;
    public float maxLife = 10;
    public float speed;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, maxLife);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (bounceCount <= 0)
        {
            Destroy(gameObject);
        }


        Vector2 random = Random.insideUnitCircle.normalized;
        Vector3 dir = new Vector3(random.x, 0, random.y).normalized;
        rb.linearVelocity = dir * speed;
        bounceCount -= 1;
    }
}
