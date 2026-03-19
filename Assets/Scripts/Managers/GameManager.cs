using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private FadeTransition gameOverTransition;
    [SerializeField] private FadeTransition winTransition;

    private bool hasWon;
    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    public GameState CurrentState { get; private set; }

    [Header("Objectives")]
    public Objective[] objectives;

    int currentObjectiveIndex = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartObjective();
    }

    private void Update()
    {
        // todo: DEBUG DAMAGE PLAYER
        if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame && !hasWon)
        {
            hasWon = true;
            PlayerInputHandler[] activeObjects = FindObjectsByType<PlayerInputHandler>(FindObjectsSortMode.None);
            foreach (var item in activeObjects)
            {
                item.enabled = false;
            }
            WinGame();
        }
    }

    void StartObjective()
    {
        if (objectives.Length == 0) return;

        if (currentObjectiveIndex >= objectives.Length)
        {
            WinGame();
            return;
        }

        objectives[currentObjectiveIndex].Begin();
    }

    public void CompleteObjective()
    {
        currentObjectiveIndex++;

        StartObjective();
    }

    public void LoseGame()
    {
        CurrentState = GameState.Lose;

        Debug.Log("Game Over");

        // Launch transition
        TransitionManager.Instance.FadeInCurrentScene(
            gameOverTransition,
            MenuManager.Instance.GetGameOverMenu(),
            0f
        );
    }

    public void WinGame()
    {
        CurrentState = GameState.Win;

        Debug.Log("Victory");

        // Launch transition
        TransitionManager.Instance.FadeInCurrentScene(
            winTransition,
            MenuManager.Instance.GetEndMenu(),
            0f
        );

        
        UIManager.Instance.ShowWin();
    }
}