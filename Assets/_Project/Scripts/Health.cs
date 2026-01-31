using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public bool destroyOnDeath = true;
    public int currentHealth;
    public bool isInvicible;

    public UnityEvent<GameObject> onDamage;
    public UnityEvent onDeath;

    bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, GameObject source)
    {
        if (isDead || amount <= 0 || isInvicible)
            return;

        currentHealth -= amount;
        onDamage?.Invoke(source);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void ResetHealth()
    {
        isDead = false;
        isInvicible = false;
        currentHealth = maxHealth;
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        onDeath?.Invoke();

        if (destroyOnDeath)
            Destroy(gameObject);
    }
}
