using UnityEngine;

namespace Spawner
{
    public struct SpawnRequest
    {
        public GameObject prefab;
        public Vector3 localOffset; // offset relatif au centre de la formation

        public SpawnRequest(GameObject prefab, Vector3 localOffset)
        {
            this.prefab = prefab;
            this.localOffset = localOffset;
        }
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
}