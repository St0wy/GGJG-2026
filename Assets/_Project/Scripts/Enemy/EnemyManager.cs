using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private bool paused;

    GameManager manager;
    public bool IsPaused => paused;
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
            e.SetPaused(paused); // applique l’état courant
        }
    }

    public void Unregister(EnemyController e)
    {
        if (e == null) return;
        enemies.Remove(e);
    }

    public void SetPausedAll(bool pause)
    {
        if (!manager.IsStarted)
        {
            paused = true;
        }
        else
        {
            paused = pause;
        }

            // cleanup nulls
            for (int i = enemies.Count - 1; i >= 0; i--)
                if (enemies[i] == null) enemies.RemoveAt(i);

        foreach (var e in enemies)
            e.SetPaused(paused);
    }

    public void TogglePauseAll() => SetPausedAll(!paused);
}