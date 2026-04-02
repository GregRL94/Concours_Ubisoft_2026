using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessibilityMenu : Menu
{
    [Header("Sliders")]
    public Slider playerDamageSlider;
    public Slider enemyDamageSlider;

    [Header("Navigation")]
    [SerializeField] private Selectable playerSliderSelectable;
    [SerializeField] private Selectable enemySliderSelectable;

    [Header("Extra Accessibility")]
    [Range(0.5f, 2f)] public float enemySpeedMultiplier = 1f;
    [Range(0.5f, 2f)] public float gameSpeedMultiplier = 1f;
    public bool reduceScreenShake = false;

    [Header("Controller Settings")]
    [SerializeField] private float step = 0.5f;
    [SerializeField] private float moveCooldown = 0.1f;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI playerDamageText;
    [SerializeField] private TextMeshProUGUI enemyDamageText;

    private float lastMoveTime;

    void Start()
    {
        SetupSlider(playerDamageSlider);
        SetupSlider(enemyDamageSlider);

        // Init valeurs
        playerDamageSlider.value = AccessibilityManager.Instance.playerDamageDealtMultiplier;
        enemyDamageSlider.value = AccessibilityManager.Instance.enemyDamageDealtMultiplier;

        // Listeners
        playerDamageSlider.onValueChanged.AddListener(OnPlayerDamageChanged);
        enemyDamageSlider.onValueChanged.AddListener(OnEnemyDamageChanged);

        // Text multiplier 
        UpdatePlayerText(AccessibilityManager.Instance.playerDamageDealtMultiplier);
        UpdateEnemyText(AccessibilityManager.Instance.enemyDamageDealtMultiplier);
    }


    void OnEnable()
    {
        MenuInputListener.UINavigate += HandleSliderNavigate;

        // Check si on est dans MainMenu
        if (!MenuManager.Instance.IsMainMenuScene())
        {
            Time.timeScale = 0f;
        }
    }

    void OnDisable()
    {
        MenuInputListener.UINavigate -= HandleSliderNavigate;

        if (!MenuManager.Instance.IsMainMenuScene())
        {
            Time.timeScale = 1f;
        }
    }

    void SetupSlider(Slider slider)
    {
        slider.minValue = 1f;
        slider.maxValue = 3f;
        slider.wholeNumbers = false;
    }

    // CONTROLLER INPUT
    void HandleSliderNavigate(Vector2 dir)
    {
        if (Time.unscaledTime - lastMoveTime < moveCooldown)
            return;

        if (Mathf.Abs(dir.x) < 0.5f)
            return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == playerSliderSelectable.gameObject)
        {
            ChangeSlider(playerDamageSlider, dir.x);
        }
        else if (current == enemySliderSelectable.gameObject)
        {
            ChangeSlider(enemyDamageSlider, dir.x);
        }
    }

    void ChangeSlider(Slider slider, float direction)
    {
        lastMoveTime = Time.unscaledTime;

        if (direction > 0)
            slider.value += step;
        else
            slider.value -= step;
    }

    // DAMAGE MODIFIER UPDATE
    public void OnPlayerDamageChanged(float value)
    {
        float snapped = Snap(value);

        if (playerDamageSlider.value != snapped)
            playerDamageSlider.value = snapped;

        AccessibilityManager.Instance.playerDamageDealtMultiplier = snapped;

        UpdatePlayerText(snapped);
    }

    public void OnEnemyDamageChanged(float value)
    {
        float snapped = Snap(value);

        if (enemyDamageSlider.value != snapped)
            enemyDamageSlider.value = snapped;

        AccessibilityManager.Instance.enemyDamageDealtMultiplier = snapped;

        UpdateEnemyText(snapped);
    }

    float Snap(float value)
    {
        return Mathf.Round(value * 2f) / 2f; // 0.5 steps
    }

    // TEXT UPDATE
    void UpdatePlayerText(float value)
    {
        playerDamageText.text = FormatMultiplier(value);
        playerDamageText.text = $"<b>{FormatMultiplier(value)}</b>";
    }

    void UpdateEnemyText(float value)
    {
        enemyDamageText.text = FormatMultiplier(value);
        enemyDamageText.text = $"<b>{FormatMultiplier(value)}</b>";
    }

    string FormatMultiplier(float value)
    {
        // Si entier "2x"
        if (Mathf.Approximately(value % 1f, 0f))
            return ((int)value) + "x";
        // Sinon "1.5x"
        return value.ToString("0.0") + "x";
    }
}