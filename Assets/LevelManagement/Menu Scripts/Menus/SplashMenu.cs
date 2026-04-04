using DG.Tweening;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SplashMenu : Menu
{
    [Header("Transition")]
    [SerializeField] private FadeTransition mainMenuTransition;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    [SerializeField] private CanvasGroup splashCanvas;
    [SerializeField] private RectTransform pressAnyButtonText;

    private Tween pulseTween;
    private bool hasPressed = false;

    void Start()
    {
        // PlayPulseAnimation();
    }

    void Update()
    {
        if (hasPressed) return;

        // debug keyboard
        bool keyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        // controller
        bool gamepadPressed = Gamepad.current != null &&
                              Gamepad.current.allControls
                              .Any(c => c is ButtonControl b && b.wasPressedThisFrame);

        if (keyboardPressed || gamepadPressed)
        {
            hasPressed = true;
            OnAnyButtonPressed();
        }
    }

    private void PlayPulseAnimation()
    {
        // animation pulse text
        pulseTween = pressAnyButtonText
            .DOScale(1.1f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnAnyButtonPressed()
    {
        pulseTween.Kill();

        // Petit feedback scale
        pressAnyButtonText
            .DOScale(0.9f, 0.3f)
            .SetEase(Ease.OutQuad);

        // Fade OUT

        TransitionManager.Instance.SplashToScene(
            mainMenuSceneName,
            mainMenuTransition
        );

    }


}