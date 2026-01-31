using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private bool paused;

    GameManager manager;
    public static EnemyManager Instance { get; private set; }
    private readonly List<EnemyController> enemies = new();


    public IReadOnlyList<EnemyController> Enemies => enemies;

    private void Awake()
    {
        manager = FindAnyObjectByType<GameManager>();
         
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(EnemyController e)
    {
        if (e == null) return;
        if (!enemies.Contains(e))
        {
            enemies.Add(e);
        }
    }

    public void Unregister(EnemyController e)
    {
        if (e == null) return;
        enemies.Remove(e);
    }
}