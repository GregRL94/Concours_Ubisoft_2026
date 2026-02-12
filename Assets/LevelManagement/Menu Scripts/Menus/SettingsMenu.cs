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
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Slider _uiVolumeSlider;

    [Header("Graphics")]
    [SerializeField] private TextMeshProUGUI _resolutionText;
    [SerializeField] private Image leftArrowImage;
    [SerializeField] private Image rightArrowImage;
    [SerializeField] private Color pressedColor = Color.black;
    [SerializeField] private float feedbackDuration = 0.12f;
    private Color _leftArrowOriginalColor;
    private Color _rightArrowOriginalColor;

    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private bool _isFullscreenToggle;

    [SerializeField] private Toggle _vSyncToggle;
    [SerializeField] private bool _isVSyncToggle;

    private Resolution[] _resolutions;
    private int _currentResolutionIndex;
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
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        _uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);
    }

    public void SetMasterVolume(float value)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20f);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    public void SetUIVolume(float value)
    {
        _audioMixer.SetFloat("UIVolume", Mathf.Log10(value) * 20f);
    }

    // GRAPHICS
    private void SetupGraphicsUI()
    {
        _fullscreenToggle.isOn = _isFullscreenToggle;
        _vSyncToggle.isOn = _isVSyncToggle;
    }

    private void SetupResolutions()
    {
        _resolutions = Screen.resolutions;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                _currentResolutionIndex = i;
                break;
            }
        }
        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        Resolution res = _resolutions[_currentResolutionIndex];
        _resolutionText.text = $"{res.width} x {res.height}";
    }

    public void NextResolution()
    {
        AudioManager.Instance.PlaySound("UI_Submit");

        _currentResolutionIndex++;

        if (_currentResolutionIndex >= _resolutions.Length)
            _currentResolutionIndex = 0;

        ApplyResolution();
    }

    public void PreviousResolution()
    {
        AudioManager.Instance.PlaySound("UI_Submit");

        _currentResolutionIndex--;

        if (_currentResolutionIndex < 0)
            _currentResolutionIndex = _resolutions.Length - 1;

        ApplyResolution();
    }

    private void ApplyResolution()
    {
        Resolution res = _resolutions[_currentResolutionIndex];
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
        Debug.Log("Woow two request on the same function from others script MAGNIFIQUE EN BOUCLE !");
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
