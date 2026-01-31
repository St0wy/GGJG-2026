using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int maxHealth = 5;
    public bool destroyOnDeath = true;

    public int CurrentHealth { get; private set; }

    public UnityEvent onDamage;
    public UnityEvent onDeath;

    bool isDead;

    void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead || amount <= 0)
            return;

        CurrentHealth -= amount;
        onDamage?.Invoke();

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
    }

    public void ResetHealth()
    {
        isDead = false;
        CurrentHealth = maxHealth;
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
