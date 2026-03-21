using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Transition Prefabs")]
    [SerializeField] private FadeTransition gameOverTransition;
    [SerializeField] private FadeTransition winTransition;
    [SerializeField] private FadeTransition nextLevelTransition;

    [Header("Scenes")]
    [SerializeField] private string[] levelScenes;

    [Header("Objectives")]
    public Objective[] objectives;

    [Header("Music Playlist")]
    public string[] playlistMusic;

    public enum GameState
    {
        Playing,
        Transition,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; }

    private int currentObjectiveIndex = 0;
    private bool hasWon = false;

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
        AudioManager.Instance.PlayRandomPlaylist(playlistMusic);
        StartObjective();

        //lastMusicIndex = Random.Range(0, musicPlaylist.Length);
        //Debug.Log("Number of music tracks in playlist: " + musicPlaylist.Length);
    }

    //  OBJECTIVES 
    void StartObjective()
    {
        if (currentObjectiveIndex >= objectives.Length)
        {
            WinGame();
            return;
        }

        CurrentState = GameState.Playing;

        objectives[currentObjectiveIndex].Begin();
    }

    public void CompleteObjective()
    {
        if (CurrentState != GameState.Playing) return;

        CurrentState = GameState.Transition;

        MissionAccomplished();
    }

    //  TRANSITIONS 

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
        CurrentState = GameState.Lose;

        TransitionManager.Instance.FadeInCurrentScene(
            gameOverTransition,
            MenuManager.Instance.GetGameOverMenu(),
            0f
        );
    }

    public void WinGame()
    {
        CurrentState = GameState.Win;

        TransitionManager.Instance.FadeInCurrentScene(
            winTransition,
            MenuManager.Instance.GetEndMenu(),
            0f
        );
    }

    //  NEXT LEVEL 
    public void LoadNextLevel()
    {
        print("next lvl");
        currentObjectiveIndex++;

        if (currentObjectiveIndex >= levelScenes.Length)
        {
            WinGame();
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(levelScenes[currentObjectiveIndex]);
        print(levelScenes[currentObjectiveIndex]);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        StartObjective();
    }

    // DEBUG 
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame && !hasWon)
        {
            hasWon = true;

            var players = FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None);
            foreach (var p in players)
                p.enabled = false;

            CompleteObjective();
        }
    }
}