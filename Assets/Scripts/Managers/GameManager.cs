using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Transition Prefabs")]
    [SerializeField] private FadeTransition gameOverTransition;
    [SerializeField] private FadeTransition nextLevelTransition; // bg screen before showing menu for next level
    [SerializeField] private FadeTransition mission2Transition;
    [SerializeField] private FadeTransition mission3Transition;
    [SerializeField] private FadeTransition winTransition;

    [Header("Scenes")]
    [SerializeField] private string[] levelScenes;

    [Header("Objectives")]
    public Objective[] objectives;
    private int currentObjectiveIndex = 0;
    private bool hasWon = false;
    public int CurrentObjectiveIndex => currentObjectiveIndex;

    [Header("Music Playlist")]
    public string[] playlistMusic;

    private float timer = 0f;
    public float CurrentTime => timer;
    public enum GameplayState
    {
        Playing,
        Transition,
        Win,
        Lose
    }
    public GameplayState CurrentState { get; private set; }

    public enum GameModeState
    {
        Gameplay,
        Menu
    }
    public GameModeState CurrentModeState { get; private set; }

    public void SetModeState(GameModeState newState)
    {
        CurrentModeState = newState;
    }


    // ---------------- INIT ----------------

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MusicPlaylistStart();
        StartObjective();
    }

    public void MusicPlaylistStart() => AudioManager.Instance.PlayRandomPlaylist(playlistMusic);


    // ---------------- OBJECTIVES ----------------

    void StartObjective()
    {
        hasWon = false;

        CurrentState = GameplayState.Playing;

        objectives[currentObjectiveIndex].Begin();
    }

    public void CompleteObjective()
    {
        if (CurrentState != GameplayState.Playing) return;

        CurrentState = GameplayState.Transition;

        currentObjectiveIndex++;

        if (currentObjectiveIndex >= levelScenes.Length)
        {
            WinGame();
            return;
        }

        MissionAccomplished();
    }

    // ---------------- TRANSITIONS ----------------

    public void MissionAccomplished()
    {
        Debug.Log("Objective Completed");

        TransitionManager.Instance.FadeInCurrentScene(
            nextLevelTransition,
            MenuManager.Instance.GetNextLevelMenu(),
            0f
        );
    }

    public void LoseGame()
    {
        CurrentState = GameplayState.Lose;

        TransitionManager.Instance.FadeInCurrentScene(
            gameOverTransition,
            MenuManager.Instance.GetGameOverMenu(),
            0f
        );
    }

    public void WinGame()
    {
        CurrentState = GameplayState.Win;

        TransitionManager.Instance.FadeInCurrentScene(
            winTransition,
            MenuManager.Instance.GetEndMenu(),
            0f
        );
    }

    // ---------------- NEXT LEVEL ----------------

    public void LoadNextLevel()
    {
        string nextScene = levelScenes[currentObjectiveIndex];
        print(nextScene);

        SceneManager.sceneLoaded += OnSceneLoaded;

        if(nextScene == "Mission2")
        {
            TransitionManager.Instance.TransitionToScene(
                nextScene,
                mission2Transition,
                0f
            );
        }
        else if(nextScene == "Mission3")
        {
            TransitionManager.Instance.TransitionToScene(
                nextScene,
                mission3Transition,
                0f
            );
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        //// relancer la playlist après reload
        //AudioManager.Instance.PlayRandomPlaylist(playlistMusic);

        StartObjective();
    }


    // ---------------- TIMER ----------------


    void Update()
    {
        if (CurrentState == GameplayState.Playing)
        {
            timer += Time.deltaTime;

            UpdateTimerUI();
        }

        // DEBUG
        if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame && !hasWon)
        {
            hasWon = true;

            var players = FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None);
            foreach (var p in players)
                p.enabled = false;

            CompleteObjective();
        }
    }

    void UpdateTimerUI()
    {
        if (UIManager.Instance == null) return;

        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        int ms = (int)((timer * 100) % 100);

        string display = $"{minutes:D2}:{seconds:D2}:{ms:D2}";

        UIManager.Instance.UpdateTimer(display);
    }



}