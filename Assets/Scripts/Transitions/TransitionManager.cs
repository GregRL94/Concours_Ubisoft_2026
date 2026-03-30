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
}
