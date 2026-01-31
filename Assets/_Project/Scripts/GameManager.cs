using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Startup")]
    public float startDelay = 3f;

    [Header("Score")]
    [SerializeField] private int shardMask;

    [Header("References")]
    private WaveSpawner spawner;
    private EnemyManager enemyManager;

    [Header("Action")]
    
    private InputAction pauseAction;

    [SerializeField] private bool started;
    [SerializeField] private bool paused = false;
    [SerializeField] private bool overed = false;
    public bool IsStarted => started;
 
    public bool IsPaused => paused;
 
    public bool IsOvered => overed;

    public int ShardMask { get => shardMask; }

    [SerializeField] GameObject MainUI;
    [SerializeField] GameObject GameOverUI;

    public void ShardIncrement()
    {
        shardMask++;
    }

    public void GameOver()
    {
        MainUI.SetActive(false);
        GameOverUI.SetActive(true);
        FindAnyObjectByType<WaveSpawner>().StopSpawnWave();
        overed = true;


    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

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
        if (spawner) spawner.SpawnFirstWave();

        // Petite frame de sécurité (optionnel)
        yield return null;

        // 4️⃣ Lancement du jeu
        started = true;
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
