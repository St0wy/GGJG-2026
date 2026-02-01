using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Shard_UI : MonoBehaviour
{
    public GameManager game;

    Label SMLabel;
    Label level;

    private void OnEnable()
    {
        game = FindAnyObjectByType<GameManager>();

        var root = GetComponent<UIDocument>().rootVisualElement;
        SMLabel = root.Q<Label>("SM-label");
        level = root.Q<Label>("Lvl");
    }

    private void Update()
    {
        SMLabel.text = $"{game.ShardMask}/{game.currentShardGoal}";
        level.text = $"Level {game.currentLevel}";
    }
}