using System.Collections;
using UnityEngine;
#if UNITY_AI_NAVIGATION
using UnityEngine.AI;
#endif

public class WaveSpawner : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Waves")]
    public WaveDefinition[] waves;
    public bool loopWaves = false;
    public float timeBetweenWaves = 3f;

    [Header("Grounding")]
    public LayerMask groundMask;           // mets Terrain + sol ici
    public float raycastStartHeight = 80f; // hauteur au-dessus pour chercher le sol
    public float raycastDistance = 200f;

    [Header("Optional NavMesh")]
    public bool requireNavMeshPosition = false;
    public float navMeshSampleRadius = 2f;

    [Header("Debug")]
    public bool drawGizmos = true;

    private int waveIndex;
    private Coroutine routine;

    private void OnEnable()
    {
        if (routine == null && waves != null && waves.Length > 0)
            routine = StartCoroutine(RunWaves());
    }

    private void OnDisable()
    {
        if (routine != null) StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator RunWaves()
    {
        if (player == null)
        {
            Debug.LogError($"{name}: Player reference missing.");
            yield break;
        }

        do
        {
            var wave = waves[waveIndex];
            if (wave == null || wave.pattern == null || wave.enemyPrefabs == null || wave.enemyPrefabs.Length == 0)
            {
                Debug.LogWarning($"{name}: Wave {waveIndex} is not configured.");
            }
            else
            {
                yield return StartCoroutine(SpawnWave(wave));
            }

            waveIndex++;
            if (waveIndex >= waves.Length)
            {
                if (!loopWaves) break;
                waveIndex = 0;
            }

            if (timeBetweenWaves > 0f)
                yield return new WaitForSeconds(timeBetweenWaves);

        } while (true);
    }

    private IEnumerator SpawnWave(WaveDefinition wave)
    {
        foreach (var item in wave.pattern.BuildPlan(wave.totalToSpawn, wave.enemyPrefabs))
        {
            // pause planifiée (prefab null)
            if (item.delay > 0f)
                yield return new WaitForSeconds(item.delay);

            if (item.prefab == null)
                continue;

            if (TryFindSpawnPosition(player.position, wave, out Vector3 pos, out Quaternion rot))
            {
                Instantiate(item.prefab, pos, rot);
            }
            else
            {
                // si pas trouvé, on skip (ou tu peux réduire le minDistance, etc.)
                Debug.LogWarning($"{name}: Could not find valid spawn position for wave '{wave.name}'.");
            }
        }
    }

    private bool TryFindSpawnPosition(Vector3 playerPos, WaveDefinition wave, out Vector3 position, out Quaternion rotation)
    {
        for (int i = 0; i < wave.maxTriesPerSpawn; i++)
        {
            // point aléatoire dans un anneau (min/max distance)
            Vector2 circle = Random.insideUnitCircle.normalized;
            float dist = Random.Range(wave.minDistanceFromPlayer, wave.maxDistanceFromPlayer);
            Vector3 candidateXZ = playerPos + new Vector3(circle.x, 0f, circle.y) * dist;

            // Raycast du haut vers le bas pour tomber sur le terrain
            Vector3 rayStart = candidateXZ + Vector3.up * raycastStartHeight;
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
                continue;

            Vector3 groundPos = hit.point;

            // Option NavMesh (si tes ennemis utilisent NavMeshAgent)
            if (requireNavMeshPosition)
            {
#if UNITY_AI_NAVIGATION
                if (!NavMesh.SamplePosition(groundPos, out NavMeshHit navHit, navMeshSampleRadius, NavMesh.AllAreas))
                    continue;

                groundPos = navHit.position;
#else
                // si le package NavMesh n'est pas dispo, on ignore
                continue;
#endif
            }

            // Sécurité finale: check distance min
            Vector3 flat = groundPos - playerPos; flat.y = 0f;
            if (flat.magnitude < wave.minDistanceFromPlayer)
                continue;

            position = groundPos;

            // rotation: regarde vers le joueur sur Y uniquement
            Vector3 look = playerPos - position; look.y = 0f;
            rotation = (look.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(look.normalized, Vector3.up) : Quaternion.identity;

            return true;
        }

        position = default;
        rotation = default;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || player == null || waves == null || waves.Length == 0) return;

        var w = waves[Mathf.Clamp(waveIndex, 0, waves.Length - 1)];
        if (w == null) return;

        Gizmos.DrawWireSphere(player.position, w.minDistanceFromPlayer);
        Gizmos.DrawWireSphere(player.position, w.maxDistanceFromPlayer);
    }
}
