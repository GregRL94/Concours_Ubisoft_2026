using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuRoot : MonoBehaviour
{
    private static MenuRoot instance;

    public Camera menuCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (MenuManager.Instance.IsMainMenuScene())
        {
            AssignMenuCamera();
        }
    }

    private void AssignMenuCamera()
    {
        // Trouver la caméra ds scene MainMenu
        menuCamera = Camera.main;

        if (menuCamera == null)
            menuCamera = FindAnyObjectByType<Camera>();
        
        if (menuCamera == null)
        {
            Debug.LogWarning("MenuRoot: No camera found!");
            return;
        }

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("MenuRoot: No Canvas on MenuRoot!");
            return;
        }

        canvas.worldCamera = menuCamera;
    }
}