using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Shard_UI : MonoBehaviour
{
    public GameManager game;

    Label SMLabel;

    private void OnEnable()
    {
        game = FindAnyObjectByType<GameManager>();

        var root = GetComponent<UIDocument>().rootVisualElement;
        SMLabel = root.Q<Label>("SM-label");
    }

    private void Update()
    {
        SMLabel.text = game.ShardMask.ToString();
    }
}