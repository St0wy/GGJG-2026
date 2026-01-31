using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/Wave Pattern/Burst")]
public class WavePattern_Burst : WavePattern
{
    public int burstSize = 5;
    public float timeBetweenSpawnsInsideBurst = 0.08f;
    public float timeBetweenBursts = 1.5f;

    public override IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        int remaining = totalToSpawn;

        while (remaining > 0)
        {
            int current = Mathf.Min(burstSize, remaining);

            for (int i = 0; i < current; i++)
                yield return new SpawnPlanItem(timeBetweenSpawnsInsideBurst, prefabs[Random.Range(0, prefabs.Length)]);

            remaining -= current;

            if (remaining > 0)
                yield return new SpawnPlanItem(timeBetweenBursts, null); // "pause" (prefab null = no spawn)
        }
    }
}
