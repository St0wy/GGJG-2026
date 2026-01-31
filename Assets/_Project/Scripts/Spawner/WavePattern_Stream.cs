using Spawner;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/Wave Pattern/Stream (Batch)")]
public class WavePattern_Stream : WavePattern
{
    public float interval = 0.5f;

    public override IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        for (int i = 0; i < totalToSpawn; i++)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Length)];

            // batch de 1 ennemi, offset = 0 (le spawner le place au centre)
            var batch = new[]
            {
                new SpawnRequest(prefab, Vector3.zero)
            };

            yield return new SpawnPlanItem(interval, batch);
        }
    }
}