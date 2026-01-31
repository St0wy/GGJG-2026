using Spawner;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/Wave Pattern/Burst (Batch)")]
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

            // Spawns rapides (1 par 1)
            for (int i = 0; i < current; i++)
            {
                var prefab = prefabs[Random.Range(0, prefabs.Length)];

                var batch = new[]
                {
                    new SpawnRequest(prefab, Vector3.zero)
                };

                yield return new SpawnPlanItem(timeBetweenSpawnsInsideBurst, batch);
            }

            remaining -= current;

            // Pause entre bursts
            if (remaining > 0 && timeBetweenBursts > 0f)
            {
                yield return new SpawnPlanItem(timeBetweenBursts, new SpawnRequest[0]); // batch vide = wait only
            }
        }
    }
}