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
    [SerializeField] private RectTransform pressAnyButtonText;

    private bool hasPressed = false;

    void Start()
    {

    }

    void Update()
    {
        if (hasPressed) return;

        // Keyboard check
        bool keyboardPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;

        // Gamepad buttons only (ignore joysticks)
        bool gamepadButtonPressed = false;
        if (Gamepad.current != null)
        {
            // On check uniquement les boutons, pas les axes
            gamepadButtonPressed = Gamepad.current.allControls
                .OfType<ButtonControl>()
                .Any(b => b.wasPressedThisFrame);
        }

        // Si clavier ou bouton gamepad pressé
        if (keyboardPressed || gamepadButtonPressed)
        {
            hasPressed = true;
            PlayPunchAnimationThenTransition();
        }
    }

    private void PlayPunchAnimationThenTransition()
    {
        if (pressAnyButtonText != null)
        {
            // Punch scale rapide pour feedback
            pressAnyButtonText.DOPunchScale(
                Vector3.one * 1.2f, // amplitude du punch
                0.5f,               // durée
                1,                  // vibrato
                0.5f                // elasticité
            ).OnComplete(() =>
            {
                // Une fois l'animation finie -> transition
                OnAnyButtonPressed();
            });
        }
        else
        {
            // fallback si pas de texte
            OnAnyButtonPressed();
        }
    }

    private void OnAnyButtonPressed()
    {
        TransitionManager.Instance.SplashToScene(
            mainMenuSceneName,
            mainMenuTransition,
            1f // durée fade, par exemple
        );
    }

    // Optionnel : pulse continu du texte
    //private void PlayPulseAnimation()
    //{
    //    pulseTween = pressAnyButtonText
    //        .DOScale(1.1f, 1f)
    //        .SetEase(Ease.InOutSine)
    //        .SetLoops(-1, LoopType.Yoyo);
    //}
}