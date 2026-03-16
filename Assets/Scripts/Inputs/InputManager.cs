using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public PlayerMapActions Inputs;

    [SerializeField] private InputSystemUIInputModule UIInputModule;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        if (Inputs == null)
        {
            Inputs = new PlayerMapActions();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIInputModule = FindAnyObjectByType<InputSystemUIInputModule>();
    }

    public void EnableAll()
    {
        Inputs.UI.Enable();
        Inputs.Player.Enable();

        if (UIInputModule)
            UIInputModule.enabled = true;
    }

    public void DisableAll()
    {
        Inputs.UI.Disable();
        Inputs.Player.Disable();

        if (UIInputModule)
            UIInputModule.enabled = false;
    }



}
