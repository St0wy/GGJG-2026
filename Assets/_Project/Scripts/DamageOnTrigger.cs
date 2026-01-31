using UnityEngine;

public class DamageOnTrigger : MonoBehaviour
{
    public int damageAmount = 1;
    public string tagToHurt;
    public bool destroyOnDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damageAmount);
            if (destroyOnDamage) Destroy(gameObject);
        }
    }

}
