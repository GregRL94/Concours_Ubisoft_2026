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
        // 1. Block inputs
        InputManager.Instance.DisableAll();

        // 2. Spawn transition
        FadeTransition transition = Instantiate(transitionPrefab);

        // 3. Fade in
        yield return transition.PlayIn().WaitForCompletion();

        yield return new WaitForSeconds(pauseDelay);
        
        // 4. Load scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 5. Fade out
        yield return transition.PlayOut().WaitForCompletion();

        // 6. Re-enable inputs on the next scene
        InputManager.Instance.EnableAll();
    }


    // Transition to fade in on the same scene when lose or win.
    public void FadeInCurrentScene( FadeTransition transitionPrefab, Menu menuToOpen, float pauseDelay)
    {
        StartCoroutine(FadeInCurrentSceneRoutine(transitionPrefab, menuToOpen, pauseDelay));
    }

    private IEnumerator FadeInCurrentSceneRoutine( FadeTransition transitionPrefab, Menu menuToOpen, float pauseDelay)
    {
        // 1. Block inputs
        InputManager.Instance.DisableAll();

        // 2. Spawn persistent transition
        FadeTransition transition = Instantiate(transitionPrefab);

        // 3. Fade IN
        yield return transition.PlayIn().WaitForCompletion();

        // 4. Pause
        if (pauseDelay > 0f)
            yield return new WaitForSeconds(pauseDelay);

        // 5. Open the correct menu 
        if (menuToOpen != null)
            MenuManager.Instance.OpenMenu(menuToOpen);

        // 6. Transition cleanup
        Destroy(transition.gameObject);

        // 7. Re-enable UI inputs
        InputManager.Instance.EnableAll();
    }

    // fade in into another menu so the current menu goes fade out 1 to 0 and the new menu goes to 1 smooth  avec param ds la methode
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

        // Fade OUT current menu
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
