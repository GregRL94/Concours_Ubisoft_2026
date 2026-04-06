using System;
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

    [Header("UI Background")]
    [SerializeField] private GameObject accessibilityBG;

    [Header("Toggles")]
    [SerializeField] private Toggle crtToggle;
    [SerializeField] private Toggle aimModeToggle;

    private float lastMoveTime;

    public static Action<bool> OnSetAdvancedShooting; // event pour activer/désactiver le tir avancé, bool = enabled/disabled


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

        crtToggle.isOn = AccessibilityManager.Instance.IsCRTEffectsEnabled();
        crtToggle.onValueChanged.AddListener(SetOnCRTToggle);

        aimModeToggle.isOn = AccessibilityManager.Instance.GetAimMode();
        aimModeToggle.onValueChanged.AddListener(SetAdvancedShooting);
        // Text multiplier 
        UpdatePlayerText(AccessibilityManager.Instance.playerDamageDealtMultiplier);
        UpdateEnemyText(AccessibilityManager.Instance.enemyDamageDealtMultiplier);


    }


    void OnEnable()
    {
        MenuInputListener.UINavigate += HandleSliderNavigate;

        // Behaviour en mode Gameplay
        if (!MenuManager.Instance.IsMainMenuScene())
        {
            Time.timeScale = 0f;
            accessibilityBG.SetActive(true);
        }
        else // Behaviour en mode MainMenu
        {
            accessibilityBG.SetActive(false);
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
        slider.minValue = 0.5f;
        slider.maxValue = 2f;
        slider.wholeNumbers = false;

        // default start
        slider.value = 1f;
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
        //playerDamageText.text = FormatMultiplierDifficulty(value);
        playerDamageText.text = $"<b>{FormatMultiplierPlayerDifficulty(value)}</b>";
    }

    void UpdateEnemyText(float value)
    {
        //enemyDamageText.text = FormatMultiplierDifficulty(value);
        enemyDamageText.text = $"<b>{FormatMultiplierEnemyDifficulty(value)}</b>";
    }

    // Code de couleur de difficulté avec du CSS 
    string FormatMultiplierPlayerDifficulty(float value)
    {
        if (Mathf.Approximately(value, 1f))
            return "<color=white>1x Normal</color>";

        if (value < 1f)
            return $"<color=red>{value:0.0}x Hard</color>";

        return $"<color=green>{value:0.0}x Easy</color>";
    }
    string FormatMultiplierEnemyDifficulty(float value)
    {
        if (Mathf.Approximately(value, 1f))
            return "<color=white>1x Normal</color>";

        if (value < 1f)
            return $"<color=green>{value:0.0}x Easy</color>";

        return $"<color=red>{value:0.0}x Hard</color>";
    }


    //BUTTON CLICKS
    public void SetOnCRTToggle(bool value)
    {
        AccessibilityManager.Instance.SetCRTEffects(value);
        AudioManager.Instance.PlaySound("UI_Submit");
    }

    public void SetAdvancedShooting(bool enabled)
    {
        AccessibilityManager.Instance.SetAimMode(enabled);
        OnSetAdvancedShooting?.Invoke(enabled);
    }


}