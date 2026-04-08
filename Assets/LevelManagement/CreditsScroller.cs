using UnityEngine;
using UnityEngine.InputSystem;

public class CreditsScroller : MonoBehaviour
{
    [Header("Configuration du scroll")]
    public float scrollSpeed = 50f;

    [Tooltip("Multiplicateur quand le joueur maintient le bouton")]
    public float fastForwardMultiplier = 3f;

    public RectTransform creditsText;

    private Vector2 startPosition;
    private bool isScrolling = false;

    private void Awake()
    {
        if (creditsText == null)
        {
            Debug.LogError("CreditsScroller: Aucun RectTransform assigné !");
            enabled = false;
            return;
        }

        startPosition = creditsText.anchoredPosition;
    }

    private void OnEnable()
    {
        creditsText.anchoredPosition = startPosition;
        isScrolling = true;
    }

    private void OnDisable()
    {
        isScrolling = false;
        creditsText.anchoredPosition = startPosition;
    }

    private void Update()
    {
        if (!isScrolling) return;

        float currentSpeed = scrollSpeed;

        // Input manettes + clavier
        bool isHoldingFast = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.buttonSouth.isPressed)
            {
                isHoldingFast = true;
                break;
            }
        }

        if (isHoldingFast)
        {
            currentSpeed *= fastForwardMultiplier;
        }

        // Scroll
        creditsText.anchoredPosition += Vector2.up * currentSpeed * Time.deltaTime;
    }
}