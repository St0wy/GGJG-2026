using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public Health playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!playerHealth)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerHealth = player.GetComponent<Health>();
        }

        if (playerHealth) playerHealth.onDeath.AddListener(OnDeath);
    }

    private void OnDeath()
    {
        Debug.Log("Game over lol");
    }
}
