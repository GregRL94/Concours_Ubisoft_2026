using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Transition Prefabs")]
    [SerializeField] private FadeTransition gameOverTransition;
    [SerializeField] private FadeTransition nextLevelTransition;
    [SerializeField] private FadeTransition mission2Transition;
    [SerializeField] private FadeTransition mission3Transition;
    [SerializeField] private FadeTransition winTransition;

    [Header("Scenes")]
    [SerializeField] private string[] levelScenes;

    [Header("Objectives Texts")]
    [SerializeField] private string[] objectiveTexts; // index correspond à levelScenes

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Music Playlist")]
    public string[] playlistMusic;

    private int currentObjectiveIndex = 0;
    public int CurrentObjectiveIndex => currentObjectiveIndex;
    private float timer = 0f;
    public float CurrentTime => timer;
    private float checkpointTime = 0f; // checkpoint du début de la mission 2+
    private bool hasWon = false;

    public enum GameplayState { Playing, Transition, Win, Lose }
    public GameplayState CurrentState { get; private set; }

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
        // On récupère l'index de la scène actuelle
        currentObjectiveIndex = 0;

        // Checkpoint = 0 pour la mission 1, sinon garder l'ancien si restart
        checkpointTime = (currentObjectiveIndex == 0) ? 0f : checkpointTime;
        timer = checkpointTime;
        hasWon = false;
        CurrentState = GameplayState.Playing;

        UpdateUI();
        MusicPlaylistStart();
    }

    void Update()
    {
        if (CurrentState != GameplayState.Playing) return;

        timer += Time.deltaTime;
        UpdateTimerUI();

        // DEBUG: passer l'objectif avec 0
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.digit0Key.wasPressedThisFrame &&
            !hasWon)
        {
            hasWon = true;
            CompleteObjective();
        }
    }

    #region TIMER & UI
    private void UpdateUI()
    {
        UpdateMissionUI();
        UpdateObjectiveUI();
        UpdateTimerUI();
    }

    private void UpdateMissionUI()
    {
        if (missionText != null)
            missionText.text = $"Mission {currentObjectiveIndex + 1}";
    }

    private void UpdateObjectiveUI()
    {
        if (objectiveText != null && objectiveTexts.Length > currentObjectiveIndex)
            objectiveText.text = objectiveTexts[currentObjectiveIndex];
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        int ms = (int)((timer * 100) % 100);
        timerText.text = $"{minutes:D2}:{seconds:D2}:{ms:D2}";
    }
    #endregion

    #region MUSIC
    public void MusicPlaylistStart()
    {
        if (playlistMusic != null && playlistMusic.Length > 0)
            AudioManager.Instance.PlayRandomPlaylist(playlistMusic);
    }
    #endregion

    #region OBJECTIVES
    public void CompleteObjective()
    {
        if (CurrentState != GameplayState.Playing) return;

        CurrentState = GameplayState.Transition;
        currentObjectiveIndex++;

        if (currentObjectiveIndex >= levelScenes.Length)
        {
            WinGame();
        }
        else
        {
            // Pour missions 2+, on sauvegarde le checkpoint au début de la mission
            if (currentObjectiveIndex > 0)
                checkpointTime = timer;

            MissionAccomplished();
        }
    }
    #endregion

    #region TRANSITIONS
    public void MissionAccomplished()
    {
        Debug.Log("Objective Completed");
        TransitionManager.Instance.FadeInCurrentScene(nextLevelTransition, MenuManager.Instance.GetNextLevelMenu(), 0f);
    }

    public void LoseGame()
    {
        CurrentState = GameplayState.Lose;
        TransitionManager.Instance.FadeInCurrentScene(gameOverTransition, MenuManager.Instance.GetGameOverMenu(), 0f);
    }

    public void WinGame()
    {
        CurrentState = GameplayState.Win;
        TransitionManager.Instance.FadeInCurrentScene(winTransition, MenuManager.Instance.GetEndMenu(), 0f);
    }
    #endregion

    #region LEVEL MANAGEMENT
    public void LoadNextLevel()
    {
        if (currentObjectiveIndex >= levelScenes.Length) return;

        string nextScene = levelScenes[currentObjectiveIndex];
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (nextScene == "Mission2")
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission2Transition, 0f);
        }
        else if (nextScene == "Mission3")
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission3Transition, 0f);
        }

        hasWon = false;
        CurrentState = GameplayState.Playing;
    }

    public void RestartLevel()
    {
        // Stop et restart musique
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("UI_Submit");
        MusicPlaylistStart();

        // Reset timer au checkpoint si mission 2+, sinon 0 pour mission 1
        timer = (currentObjectiveIndex == 0) ? 0f : checkpointTime;
        hasWon = false;
        CurrentState = GameplayState.Playing;

        SceneManager.sceneLoaded += OnSceneReloaded;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void OnSceneReloaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneReloaded;
        UpdateUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UpdateUI();
    }

    //private int GetSceneIndex(string sceneName)
    //{
    //    for (int i = 0; i < levelScenes.Length; i++)
    //    {
    //        if (levelScenes[i] == sceneName)
    //            return i;
    //    }
    //    return 0; // défaut = mission 1
    //}
    #endregion
}