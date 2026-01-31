using System;
using UnityEngine;
using UnityEngine.Events;

public class GameOverHandler : MonoBehaviour
{
    public Health playerHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerHealth = player.GetComponent<Health>();
        if (playerHealth) playerHealth.onDeath.AddListener(OnDeath);
        if (playerHealth) playerHealth.destroyOnDeath = true;
    }

    private void OnDeath()
    {
        Debug.Log("Game over lol");
    }
}
