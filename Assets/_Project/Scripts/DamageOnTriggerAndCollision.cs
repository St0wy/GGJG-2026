using UnityEngine;

public class DamageOnTriggerAndCollision : MonoBehaviour
{
    public int damageAmount = 1;
    public string tagToHurt;
    public bool destroyOnDamage;

    private void OnTriggerEnter(Collider other)
    {
        HandleCollision(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.collider);
    }

    void HandleCollision(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            if (!health.CompareTag(tagToHurt)) return;
            health.TakeDamage(damageAmount, gameObject);
            if (destroyOnDamage) Destroy(gameObject);
        }
    }
}
