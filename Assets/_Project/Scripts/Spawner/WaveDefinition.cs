using UnityEngine;

[CreateAssetMenu(menuName = "Spawner/Wave Definition")]
public class WaveDefinition : ScriptableObject
{
    [Header("Wave")]
    public WavePattern pattern;
    public int totalToSpawn = 20;

    [Header("Enemies")]
    public GameObject[] enemyPrefabs;

    [Header("Safety / Range")]
    public float minDistanceFromPlayer = 12f;  // sécurité
    public float maxDistanceFromPlayer = 30f;  // zone de spawn autour du joueur

    [Header("Spawn Attempts")]
    public int maxTriesPerSpawn = 25;
}