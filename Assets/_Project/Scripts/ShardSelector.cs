using UnityEngine;

public class ShardSelector : MonoBehaviour
{
    public MeshFilter meshFilter;
    public Mesh[] shards;

    private void Awake()
    {
        Mesh mesh = shards[Random.Range(0, shards.Length)];
        meshFilter.mesh = mesh;
    }
}
