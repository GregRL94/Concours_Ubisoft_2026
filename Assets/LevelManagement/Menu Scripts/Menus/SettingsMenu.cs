using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenu : Menu
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;

    [Header("Graphics")]
    [SerializeField] private TextMeshProUGUI resolutionText;
    [SerializeField] private Image leftArrowImage;
    [SerializeField] private Image rightArrowImage;
    [SerializeField] private Color pressedColor = Color.black;
    [SerializeField] private float feedbackDuration = 0.12f;
    private Color _leftArrowOriginalColor;
    private Color _rightArrowOriginalColor;


    [Header("Audio Settings (dB)")]
    [SerializeField] private float maxDb = -6f; // slider à 1 à environ -6 dB
    //[SerializeField] private float minDb = -80f;
    [SerializeField] private float startSliderValue = 1f;

    [Header("Fullscreen Toggle")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private bool isFullscreenToggle;

    [Header("VSync Toggle")]
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private bool isVSyncToggle;

    private Resolution[] resolutions;
    private int currentResolutionIndex;
    [SerializeField] private Selectable sideArrowSelectable; // side scroll resolutions


    private void Awake()
    {
        SetupResolutions();
        SetupGraphicsUI();

        SetupAudioUI();
        InitializeVolumeSliders();
    }

    private void Start()
    {
        _leftArrowOriginalColor = leftArrowImage.color;
        _rightArrowOriginalColor = rightArrowImage.color;
    }

    // =========================
    // AUDIO SETUP
    // =========================

    private void SetupAudioUI()
    {
        SetupSlider(masterVolumeSlider);
        SetupSlider(musicVolumeSlider);
        SetupSlider(sfxVolumeSlider);
        SetupSlider(uiVolumeSlider);

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);
    }

    private void SetupSlider(Slider slider)
    {
        slider.minValue = 0.0001f; 
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
    }


    private void InitializeVolumeSliders()
    {
        float startValue = startSliderValue;

        masterVolumeSlider.value = startValue;
        musicVolumeSlider.value = startValue;
        sfxVolumeSlider.value = startValue;
        uiVolumeSlider.value = startValue;

        SetMasterVolume(startValue);
        SetMusicVolume(startValue);
        SetSFXVolume(startValue);
        SetUIVolume(startValue);
    }


    private float LinearToDb(float value)
    {
        float db = Mathf.Log10(value) * 20f;

        // offset pour que 1 = maxDb au lieu de 0
        return db + maxDb;
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", LinearToDb(value));
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", LinearToDb(value));
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", LinearToDb(value));
    }

    public void SetUIVolume(float value)
    {
        audioMixer.SetFloat("UIVolume", LinearToDb(value));
    }

    // GRAPHICS
    private void SetupGraphicsUI()
    {
        fullscreenToggle.isOn = isFullscreenToggle;
        vSyncToggle.isOn = isVSyncToggle;
    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }
        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        Resolution res = resolutions[currentResolutionIndex];
        resolutionText.text = $"{res.width} x {res.height}";
    }

    public void NextResolution()
    {
        AudioManager.Instance.PlaySound("UI_Submit");

        currentResolutionIndex++;

        if (currentResolutionIndex >= resolutions.Length)
            currentResolutionIndex = 0;

        ApplyResolution();
    }

    public void PreviousResolution()
    {
        AudioManager.Instance.PlaySound("UI_Submit");

        currentResolutionIndex--;

        if (currentResolutionIndex < 0)
            currentResolutionIndex = resolutions.Length - 1;

        ApplyResolution();
    }

    private void ApplyResolution()
    {
        Resolution res = resolutions[currentResolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        UpdateResolutionText();
    }

    private IEnumerator ArrowFeedback(Image arrowImage, Color originalColor)
    {
        arrowImage.color = pressedColor;
        yield return new WaitForSeconds(feedbackDuration);
        arrowImage.color = originalColor;
    }


    private void OnEnable()
    {
        MenuInputListener.UINavigate += HandleResolutionNavigate;

        // Check si on est dans MainMenu
        if (!MenuManager.Instance.IsMainMenuScene())
        {
            // Freeze le gameplay
            Time.timeScale = 0f;
            //InputManager.Instance.DisableGameplay();
        }
    }

    private void OnDisable()
    {
        MenuInputListener.UINavigate -= HandleResolutionNavigate;

        if (!MenuManager.Instance.IsMainMenuScene())
        {
            // Reprendre le gameplay
            Time.timeScale = 1f;
            //InputManager.Instance.EnableGameplay();
        }
    }
    private void HandleResolutionNavigate(Vector2 dir)
        {
            if (EventSystem.current.currentSelectedGameObject != sideArrowSelectable.gameObject)
                return;

            if (dir.x > 0.5f)
            {
                NextResolution();
                StartCoroutine(ArrowFeedback(rightArrowImage, _rightArrowOriginalColor));
            }
            else if (dir.x < -0.5f)
            {
                PreviousResolution();
                StartCoroutine(ArrowFeedback(leftArrowImage, _leftArrowOriginalColor));
            }
        }


    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        AudioManager.Instance.PlaySound("UI_Submit");
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        AudioManager.Instance.PlaySound("UI_Submit");
    }


    // BUTTON
    public override void OnBack()
    {
        AudioManager.Instance.PlaySound("UI_Back");
        base.OnBack();
    }
}
