using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private bool isFullscreenToggle;

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
    }
    private void Start()
    {
        _leftArrowOriginalColor = leftArrowImage.color;
        _rightArrowOriginalColor = rightArrowImage.color;
    }



    // AUDIO
    private void SetupAudioUI()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);
    }

    public void SetMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    public void SetUIVolume(float value)
    {
        audioMixer.SetFloat("UIVolume", Mathf.Log10(value) * 20f);
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
        MenuInputListener.UINavigate += HandlePrintNavigate;
    }

    private void OnDisable()
    {
        MenuInputListener.UINavigate -= HandleResolutionNavigate;
        MenuInputListener.UINavigate -= HandlePrintNavigate;
    }

    private void HandleResolutionNavigate(Vector2 dir)
    {
        if (EventSystem.current.currentSelectedGameObject != sideArrowSelectable.gameObject)
            return;

        if (dir.x > 0.9f)
        {
            NextResolution();
            StartCoroutine(ArrowFeedback(rightArrowImage, _rightArrowOriginalColor));
        }
        else if (dir.x < -0.9f)
        {
            PreviousResolution();
            StartCoroutine(ArrowFeedback(leftArrowImage, _leftArrowOriginalColor));
        }
    }

    private void HandlePrintNavigate(Vector2 dir)
    {
        //Debug.Log("Woow two request on the same function");
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
