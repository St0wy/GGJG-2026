using System.Collections.Generic;
using UnityEngine;

public enum FormationShape
{
    V,
    Line,
    Rectangle,
    Cluster
}

[CreateAssetMenu(menuName = "Spawner/Wave Pattern/Formation Start (Multi)")]
public class WavePattern_FormationStart : WavePattern
{
    [Header("Formation")]
    public FormationShape shape = FormationShape.Rectangle;
    public int formationCount = 12;
    public float spacing = 2.5f;

    [Header("Rectangle settings")]
    public int rectColumns = 4; // nb colonnes (X). Les lignes se calculent automatiquement.
    public bool centerRectangle = true;

    [Header("Cluster settings")]
    public float clusterRadius = 3.5f; // rayon du groupe
    public bool clusterUniform = true; // uniforme (true) ou plus dense au centre (false)

    [Header("After formation (optional stream)")]
    public float streamInterval = 0.6f;

    public override IEnumerable<SpawnPlanItem> BuildPlan(int totalToSpawn, GameObject[] prefabs)
    {
        if (prefabs == null || prefabs.Length == 0) yield break;

        int toSpawn = Mathf.Max(0, totalToSpawn);
        int first = Mathf.Min(formationCount, toSpawn);

        if (first > 0)
        {
            var batch = BuildFormationBatch(first, prefabs);
            yield return new SpawnPlanItem(0f, batch);
            toSpawn -= first;
        }

        // Ensuite spawn classique (1 par 1 au centre)
        while (toSpawn > 0)
        {
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            yield return new SpawnPlanItem(streamInterval, new[]
            {
                new Spawner.SpawnRequest(prefab, Vector3.zero)
            });
            toSpawn--;
        }
    }

    private Spawner.SpawnRequest[] BuildFormationBatch(int count, GameObject[] prefabs)
    {
        var batch = new Spawner.SpawnRequest[count];

        switch (shape)
        {
            case FormationShape.V:
                FillV(batch, prefabs);
                break;

            case FormationShape.Line:
                FillLine(batch, prefabs);
                break;

            case FormationShape.Rectangle:
                FillRectangle(batch, prefabs);
                break;

            case FormationShape.Cluster:
                FillCluster(batch, prefabs);
                break;
        }

        return batch;
    }

    private void FillLine(Spawner.SpawnRequest[] batch, GameObject[] prefabs)
    {
        int count = batch.Length;
        float half = (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float x = (i - half) * spacing;
            batch[i] = new Spawner.SpawnRequest(Rand(prefabs), new Vector3(x, 0f, 0f));
        }
    }

    private void FillV(Spawner.SpawnRequest[] batch, GameObject[] prefabs)
    {
        int count = batch.Length;
        batch[0] = new Spawner.SpawnRequest(Rand(prefabs), Vector3.zero);

        int k = 1;
        int row = 1;

        while (k < count)
        {
            if (k < count)
                batch[k++] = new Spawner.SpawnRequest(Rand(prefabs), new Vector3(-row * spacing, 0f, -row * spacing));
            if (k < count)
                batch[k++] = new Spawner.SpawnRequest(Rand(prefabs), new Vector3(row * spacing, 0f, -row * spacing));
            row++;
        }
    }

    private void FillRectangle(Spawner.SpawnRequest[] batch, GameObject[] prefabs)
    {
        int count = batch.Length;

        int cols = Mathf.Max(1, rectColumns);
        int rows = Mathf.CeilToInt(count / (float)cols);

        // Origine du rectangle : centré ou coin (0,0)
        float xOffset = centerRectangle ? (cols - 1) * 0.5f : 0f;
        float zOffset = centerRectangle ? (rows - 1) * 0.5f : 0f;

        int k = 0;
        for (int r = 0; r < rows && k < count; r++)
        {
            for (int c = 0; c < cols && k < count; c++)
            {
                float x = (c - xOffset) * spacing;
                float z = (r - zOffset) * spacing;

                // Convention: "devant" = z négatif (comme ton V)
                batch[k++] = new Spawner.SpawnRequest(Rand(prefabs), new Vector3(x, 0f, -z));
            }
        }
    }

    private void FillCluster(Spawner.SpawnRequest[] batch, GameObject[] prefabs)
    {
        int count = batch.Length;

        for (int i = 0; i < count; i++)
        {
            Vector2 p;

            if (clusterUniform)
            {
                // Uniforme dans un disque
                p = Random.insideUnitCircle * clusterRadius;
            }
            else
            {
                // Plus dense au centre (on prend deux fois insideUnitCircle et on garde le plus petit rayon)
                Vector2 a = Random.insideUnitCircle * clusterRadius;
                Vector2 b = Random.insideUnitCircle * clusterRadius;
                p = (a.sqrMagnitude < b.sqrMagnitude) ? a : b;
            }

            batch[i] = new Spawner.SpawnRequest(Rand(prefabs), new Vector3(p.x, 0f, p.y));
        }
    }

    private static GameObject Rand(GameObject[] prefabs) => prefabs[Random.Range(0, prefabs.Length)];
}
