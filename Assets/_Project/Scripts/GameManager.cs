using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class GameManager : MonoBehaviour
{
    [Header("Startup")]
    public float startDelay = 3f;

    [Header("Score")]
    [SerializeField] private int shardMask;

    [Header("References")]
    private WaveSpawner spawner;
    private EnemyManager enemyManager;

    //[Header("Input")]
    //public InputActionReference pauseAction;
    InputAction pauseAction;

    private bool started;
    public bool IsStarted => started;

    [SerializeField] private bool paused = false;
    public bool IsPaused => paused;
    public int ShardMask { get => shardMask;}

    public void ShardIncrement()
    {
        shardMask++;
    }

    private void Awake()
    {
        if (enemyManager == null)
            enemyManager = FindAnyObjectByType<EnemyManager>();

        if (spawner == null)
            spawner = FindAnyObjectByType<WaveSpawner>();

        pauseAction = InputSystem.actions.FindAction("Pause");
    }


    private void Start()
    {
        StartCoroutine(GameStartRoutine());
    }

    private void OnEnable()
    {
        pauseAction.performed += OnPausePerformed;
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private IEnumerator GameStartRoutine()
    {
        // 2️⃣ Attente avant le début
        yield return new WaitForSeconds(startDelay);

        // 3️⃣ Spawn de la première vague
        spawner.SpawnFirstWave();

        // Petite frame de sécurité (optionnel)
        yield return null;

        // 4️⃣ Lancement du jeu
        started = true;
        enemyManager.SetPausedAll(false);
        paused = false;
    }

    public void SetPausedAll(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

            paused = pause;
        enemyManager.SetPausedAll(pause);
        
    }

    public void TogglePauseAll() => SetPausedAll(!paused);

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        // Empêche la pause avant le vrai début
        if (!started)
            return;

        TogglePauseAll();
    }
}
