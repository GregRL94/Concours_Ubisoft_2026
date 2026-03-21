using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    private static int mainMenuIndex = 0;

    public static void LoadLevel(string levelName)
    {
        if (Application.CanStreamedLevelBeLoaded(levelName))
        {
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning("GAMEMANAGER LoadLevel Error: invalid scene specified!");
        }
    }

    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified!");
        }
    }


    public static void ReloadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public static void LoadNextLevel()
    {
        int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1)
            % SceneManager.sceneCountInBuildSettings;
        nextSceneIndex = Mathf.Clamp(nextSceneIndex, mainMenuIndex, nextSceneIndex);
        LoadLevel(nextSceneIndex);
    }


    public static void LoadMainMenuLevel()
    {
        LoadLevel(mainMenuIndex);
    }


    public static bool CanLoadEndMenu() 
    {
        if(SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
            return true;
        return false;
    }

    public static bool IsGameplayScene()
    {
        return SceneManager.GetActiveScene().buildIndex >= 1;
    }

}

