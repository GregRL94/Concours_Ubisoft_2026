using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Transition from a scene to another 
    public void TransitionToScene(string sceneName, FadeTransition transitionPrefab, float pauseDelay)
    {
        StartCoroutine(TransitionRoutine(sceneName, transitionPrefab, pauseDelay));
    }

    private IEnumerator TransitionRoutine(string sceneName, FadeTransition transitionPrefab, float pauseDelay)
    {
        InputManager.Instance.DisableAll();

        FadeTransition transition = Instantiate(transitionPrefab);

        yield return transition.PlayIn()
            .SetUpdate(true)   // Permet d’ignorer Time.timeScale
            .WaitForCompletion();

        yield return new WaitForSeconds(pauseDelay);
        
        yield return SceneManager.LoadSceneAsync(sceneName);

        yield return transition.PlayOut().WaitForCompletion();

        InputManager.Instance.EnableAll();
    }


    // Transition to fade in on the same scene when lose or win.
    public void FadeInCurrentScene( FadeTransition transitionPrefab, Menu menuToOpen, float pauseDelay)
    {
        StartCoroutine(FadeInCurrentSceneRoutine(transitionPrefab, menuToOpen, pauseDelay));
    }

    private IEnumerator FadeInCurrentSceneRoutine( FadeTransition transitionPrefab, Menu menuToOpen, float pauseDelay)
    {
        InputManager.Instance.DisableAll();

        FadeTransition transition = Instantiate(transitionPrefab);

        yield return transition.PlayIn().WaitForCompletion();

        if (pauseDelay > 0f)
            yield return new WaitForSeconds(pauseDelay);

        if (menuToOpen != null)
            MenuManager.Instance.OpenMenu(menuToOpen);

        Destroy(transition.gameObject);

        InputManager.Instance.EnableAll();
    }

    public void TransitionBetweenMenus(Menu currentMenu, Menu nextMenu, float fadeOutDuration, float fadeInDuration)
    {
        StartCoroutine(TransitionBetweenMenusRoutine(currentMenu, nextMenu, fadeOutDuration, fadeInDuration));
    }

    private IEnumerator TransitionBetweenMenusRoutine(Menu currentMenu, Menu nextMenu, float fadeOutDuration, float fadeInDuration)
    {
        InputManager.Instance.DisableAll();

        CanvasGroup currentCanvas = currentMenu.GetComponent<CanvasGroup>();
        if (currentCanvas == null)
            currentCanvas = currentMenu.gameObject.AddComponent<CanvasGroup>();

        // Fade out current menu
        yield return currentCanvas
            .DOFade(0f, fadeOutDuration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
        currentMenu.gameObject.SetActive(false);
        currentCanvas.alpha = 1;

        // Open next menu
        MenuManager.Instance.OpenMenu(nextMenu);

        CanvasGroup nextCanvas = nextMenu.GetComponent<CanvasGroup>();
        if (nextCanvas == null)
            nextCanvas = nextMenu.gameObject.AddComponent<CanvasGroup>();

        nextCanvas.alpha = 0f;

        // Fade IN next menu
        yield return nextCanvas
            .DOFade(1f, fadeInDuration)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();


        InputManager.Instance.EnableAll();
    }



    // Appelle cette méthode pour passer au MainMenu
    public void SplashToScene(string sceneName, FadeTransition transitionPrefab, float duration = 1.5f)
    {
        StartCoroutine(SplashToSceneRoutine(sceneName, transitionPrefab, duration));
    }

    private IEnumerator SplashToSceneRoutine(string sceneName, FadeTransition transitionPrefab, float duration)
    {
        // 1. Instantiate transition prefab
        FadeTransition transition = Instantiate(transitionPrefab);
        DontDestroyOnLoad(transition.gameObject);

        CanvasGroup canvasGroup = transition.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = transition.gameObject.AddComponent<CanvasGroup>();

        // Ensure black screen starts fully transparent for fade IN
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        // 2. FADE IN (0 -> 1)
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            canvasGroup.alpha = t; // fade IN
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 3. LOAD SCENE ASYNC WITHOUT IMMEDIATE ACTIVATION
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait 1 frame to ensure fade IN rendered
        yield return null;

        // Wait until the scene is fully loaded (progress >= 0.9)
        while (asyncLoad.progress < 0.9f)
            yield return null;

        // Activate the scene
        asyncLoad.allowSceneActivation = true;

        // Wait 1 frame to ensure scene is active
        yield return null;

        // 4. FADE OUT (1 -> 0)
        time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / duration);
            canvasGroup.alpha = 1f - t;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // 5. Cleanup
        canvasGroup.blocksRaycasts = false;
        Destroy(transition.gameObject);
    }
    public void TransitionRestartScene(FadeTransition transitionPrefab)
    {
        StartCoroutine(RestartSceneRoutine(transitionPrefab));
    }

    private IEnumerator RestartSceneRoutine(FadeTransition transitionPrefab)
    {
        InputManager.Instance.DisableAll();

        FadeTransition transition = Instantiate(transitionPrefab);

        yield return transition.PlayIn()
            .SetUpdate(true)   // Permet d’ignorer Time.timeScale
            .WaitForCompletion();

        GameManager.Instance.RestartLevel();

        yield return transition.PlayOut().WaitForCompletion();

        InputManager.Instance.EnableAll();
    }

    //public void TransitionToMainMenuScene(string sceneName, FadeTransition transitionPrefab)
    //{
    //    StartCoroutine(MainMenuSceneRoutine(sceneName, transitionPrefab));
    //}

    //private IEnumerator MainMenuSceneRoutine(string sceneName, FadeTransition transitionPrefab)
    //{
    //    InputManager.Instance.DisableAll();

    //    FadeTransition transition = Instantiate(transitionPrefab);

    //    yield return transition.PlayIn()
    //        .SetUpdate(true)   // Permet d’ignorer Time.timeScale
    //        .WaitForCompletion();

    //    GameManager.Instance.RestartLevel();

    //    yield return transition.PlayOut().WaitForCompletion();

    //    InputManager.Instance.EnableAll();
    //}

}
