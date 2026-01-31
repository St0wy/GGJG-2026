using System.Collections.Generic;
using UnityEngine;

public abstract class WavePattern : ScriptableObject
{
    /// <summary>
    /// Generates a ‘planned’ spawn sequence based on the total requested.
    /// Each item = (delay, prefab) to be instantiated.
    /// </summary>
    public abstract IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs);
}

public struct SpawnPlanItem
{
    public float delay;
    public GameObject prefab;

    public SpawnPlanItem(float delay, GameObject prefab)
    {
        this.delay = delay;
        this.prefab = prefab;
    }
}
