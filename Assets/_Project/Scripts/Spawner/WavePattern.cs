using Spawner;
using System.Collections.Generic;
using UnityEngine;

public abstract class WavePattern : ScriptableObject
{
    public abstract IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs);
}
public struct SpawnPlanItem
{
    public float delay;
    public SpawnRequest[] batch; // si null/empty => juste attendre

    public SpawnPlanItem(float delay, SpawnRequest[] batch)
    {
        this.delay = delay;
        this.batch = batch;
    }
}