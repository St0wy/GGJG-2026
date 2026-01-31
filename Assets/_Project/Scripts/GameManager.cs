using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int shardMask;

    public int ShardMask { get => shardMask;}

    public void ShardIncrement()
    {
        shardMask++;
    }
}
