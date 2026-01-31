using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/Wave Pattern/Stream")]
public class WavePattern_Stream : WavePattern
{
    public float interval = 0.5f;

    public override IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        for (int i = 0; i < totalToSpawn; i++)
        {
            yield return new SpawnPlanItem(interval, prefabs[Random.Range(0, prefabs.Length)]);
        }
    }
}
