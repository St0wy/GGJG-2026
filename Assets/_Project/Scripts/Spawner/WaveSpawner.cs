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
            if (item.delay > 0f)
                yield return new WaitForSeconds(item.delay);

            if (item.batch == null || item.batch.Length == 0)
                continue;

            // On cherche UN centre valide pour le batch
            if (!TryFindSpawnCenter(player.position, wave, out Vector3 center, out Quaternion facing))
            {
                Debug.LogWarning($"{name}: Could not find spawn center for wave '{wave.name}'.");
                continue;
            }

            // Spawn chaque élément du batch avec offsets
            for (int i = 0; i < item.batch.Length; i++)
            {
                var req = item.batch[i];
                if (req.prefab == null) continue;

                if (TryResolveOnGround(center, facing, req.localOffset, wave, out Vector3 pos))
                {
                    // Rotation : face au joueur (yaw only)
                    Vector3 look = player.position - pos; look.y = 0f;
                    Quaternion rot = (look.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(look.normalized, Vector3.up) : Quaternion.identity;

                    Instantiate(req.prefab, pos, rot);
                }
            }
        }
    }

    private bool TryResolveOnGround(Vector3 center, Quaternion facing, Vector3 localOffset, WaveDefinition wave, out Vector3 finalPos)
    {
        // Offset en world space : la formation est orientée par "facing"
        Vector3 worldOffset = facing * localOffset;

        Vector3 candidate = center + worldOffset;

        // Raycast down pour trouver le sol au bon endroit
        Vector3 rayStart = candidate + Vector3.up * raycastStartHeight;
        if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
        {
            finalPos = default;
            return false;
        }

        finalPos = hit.point;

        // Optionnel: tu peux rajouter ici un anti-collision :
        // if (Physics.CheckSphere(finalPos + Vector3.up * 0.5f, 0.5f, obstacleMask)) return false;

        return true;
    }

    private bool TryFindSpawnCenter(Vector3 playerPos, WaveDefinition wave, out Vector3 center, out Quaternion facing)
    {
        for (int i = 0; i < wave.maxTriesPerSpawn; i++)
        {
            Vector2 circle = Random.insideUnitCircle.normalized;
            float dist = Random.Range(wave.minDistanceFromPlayer, wave.maxDistanceFromPlayer);
            Vector3 candidateXZ = playerPos + new Vector3(circle.x, 0f, circle.y) * dist;

            Vector3 rayStart = candidateXZ + Vector3.up * raycastStartHeight;
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
                continue;

            Vector3 groundPos = hit.point;

            // sécurité min
            Vector3 flat = groundPos - playerPos; flat.y = 0f;
            if (flat.magnitude < wave.minDistanceFromPlayer)
                continue;

            // rotation "facing" = regarde le joueur (yaw only)
            Vector3 dir = playerPos - groundPos; dir.y = 0f;
            facing = (dir.sqrMagnitude > 0.01f) ? Quaternion.LookRotation(dir.normalized, Vector3.up) : Quaternion.identity;

            center = groundPos;
            return true;
        }

        center = default;
        facing = default;
        return false;
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
