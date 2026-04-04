using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;
using System.Collections;
using System;


public class GameManager : MonoBehaviour
{
    public static Action<float> OnUltimateJaugeIncrease; // event pour augmenter la jauge ultimate, float = amount
    public static GameManager Instance;

    [Header("Transition Prefabs")]
    [SerializeField] private FadeTransition gameOverTransition;
    [SerializeField] private FadeTransition nextLevelTransition;
    [SerializeField] private FadeTransition mission2Transition;
    [SerializeField] private FadeTransition mission3Transition;
    [SerializeField] private FadeTransition mission4Transition;
    [SerializeField] private FadeTransition mission5Transition;
    [SerializeField] private FadeTransition winTransition;

    [Header("Scenes")]
    [SerializeField] private List<string> levelScenes;

    [Header("Objectives Texts")]
    [SerializeField] private string[] objectiveTexts; // index correspond à levelScenes

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI timerText;


    [Header("Enemy Count Text")]
    [SerializeField] private TextMeshProUGUI enemyCountText;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float durationEnemyCount = 0.1f;
    private Coroutine enemyAnimRoutine;

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

    public int AliensCount { get; private set; }

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



    public void UpdateCountsUI(int enemyCount, int spawnerCount)
    {
        AliensCount = enemyCount + spawnerCount;

        if (enemyCountText != null)
            enemyCountText.text = $"Aliens left: {AliensCount}";

        // todo: add sound cue for enemy count

        if (enemyAnimRoutine != null)
            StopCoroutine(enemyAnimRoutine);

        enemyAnimRoutine = StartCoroutine(AnimateEnemyCount());
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

        StartCoroutine(CompleteObjectiveRoutine());
        //CompleteObjectiveRoutine();
    }

    private IEnumerator CompleteObjectiveRoutine()
    {
        // Bloque direct le state
        CurrentState = GameplayState.Transition;

        // Attend que l'anim soit completement fini
        yield return StartCoroutine(ObjectiveAnimRoutine());

        currentObjectiveIndex++;

        if (currentObjectiveIndex >= levelScenes.Count)
        {
            WinGame();
        }
        else
        {
            if (currentObjectiveIndex > 0)
                checkpointTime = timer;

            MissionAccomplished();
        }
    }

    private IEnumerator ObjectiveAnimRoutine()
    {
        // UI + SFX
        objectiveText.text = "OBJECTIVE COMPLETED";
        AudioManager.Instance.PlaySound("SFX_ObjectiveCompleted");
        
        // Petite anim - scale punch 
        yield return StartCoroutine(AnimateObjectiveText());

        // Attente 2 secondes
        yield return new WaitForSeconds(1.5f);
    }

    #endregion

    #region TRANSITIONS
    public void MissionAccomplished()
    {
        //Debug.Log("Objective Completed");
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
        if (currentObjectiveIndex >= levelScenes.Count) return;

        //string nextScene = levelScenes[currentObjectiveIndex];
        string nextScene = levelScenes[currentObjectiveIndex];
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (nextScene == levelScenes[1]) // mission 2 transition
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission2Transition, 0f);
        }
        else if (nextScene == levelScenes[2]) // mission 3 transition
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission3Transition, 0f);
        }
        else if (nextScene == levelScenes[3]) // mission 4 transition
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission4Transition, 0f);
        }
        else if (nextScene == levelScenes[levelScenes.Count - 1]) // mission 4 transition
        {
            TransitionManager.Instance.TransitionToScene(nextScene, mission5Transition, 0f);
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

    private IEnumerator AnimateObjectiveText()
    {
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        float duration = 0.25f;
        float time = 0f;

        objectiveText.transform.localScale = startScale;

        // Scale up rapide
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            objectiveText.transform.localScale = Vector3.Lerp(startScale, targetScale * 1.2f, t);
            yield return null;
        }

        // Petit bounce retour
        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            objectiveText.transform.localScale = Vector3.Lerp(targetScale * 1.2f, targetScale, t);
            yield return null;
        }
    }
    #endregion

    #region INGAME EVENTS BINDINGS
    public void IncreaseUltimateJauge(float amount)
    {
        OnUltimateJaugeIncrease?.Invoke(amount);
    }

    void OnEnable()
    {
        EnemyManager.OnCountsChanged += UpdateCountsUI;
    }

    void OnDisable()
    {
        EnemyManager.OnCountsChanged -= UpdateCountsUI;
    }
    #endregion

    #region ANIMATION UI
    private IEnumerator AnimateEnemyCount()
    {
        Transform t = enemyCountText.transform;

        Vector3 baseScale = Vector3.one;
        Vector3 targetScale = Vector3.one * maxScale;

        float duration = durationEnemyCount;
        float time = 0f;

        // Scale up
        while (time < duration)
        {
            time += Time.deltaTime;
            float tLerp = time / duration;
            t.localScale = Vector3.Lerp(baseScale, targetScale, tLerp);
            yield return null;
        }

        time = 0f;

        // Scale down
        while (time < duration)
        {
            time += Time.deltaTime;
            float tLerp = time / duration;
            t.localScale = Vector3.Lerp(targetScale, baseScale, tLerp);
            yield return null;
        }

        t.localScale = baseScale;
    }
    #endregion

}