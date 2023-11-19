using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Assign - Screens")]
    [SerializeField] protected GameObject mainScreen;
    [SerializeField] protected GameObject settingsScreen;
    [SerializeField] protected GameObject creditsScreen;

    [Header("Assign - Main Screen Buttons")]
    [SerializeField] protected Button continueButton;
    [SerializeField] protected Button startButton;
    [SerializeField] protected Button settingsButton;
    [SerializeField] protected Button creditsButton;
    [SerializeField] protected Button quitButton;

    [Header("Assign - Back Buttons")]
    [SerializeField] protected Button backFromSettingsButton;
    [SerializeField] protected Button backFromCreditsButton;

    [Header("Assign - Settings Sliders")]
    [SerializeField] protected Slider masterAudioSlider;
    [SerializeField] protected Slider sfxSlider;
    [SerializeField] protected Slider musicSlider;
    [SerializeField] protected Slider sensitivitySlider;

    [Header("Assign - Settings Texts")]
    [SerializeField] protected TextMeshProUGUI masterAudioValueText;
    [SerializeField] protected TextMeshProUGUI sfxValueText;
    [SerializeField] protected TextMeshProUGUI musicValueText;
    [SerializeField] protected TextMeshProUGUI sensitivityValueText;

    [Header("Assign - Audio Mixer")] [SerializeField] protected AudioMixer audioMixer;

    protected void OnAwake()
    {
        if (continueButton != null) continueButton.onClick.AddListener(OnContinueButton);
        if (startButton != null) startButton.onClick.AddListener(OnStartButton);
        settingsButton.onClick.AddListener(OnSettingButton);
        creditsButton.onClick.AddListener(OnCreditsButton);
        quitButton.onClick.AddListener(Application.Quit);

        backFromSettingsButton.onClick.AddListener(BackToMainScreen);
        backFromCreditsButton.onClick.AddListener(BackToMainScreen);

        masterAudioSlider.onValueChanged.AddListener(ChangeMasterVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);

        masterAudioSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity") * 10f;

        masterAudioValueText.text = $"{masterAudioSlider.value + 80}";
        sfxValueText.text = $"{sfxSlider.value + 80}";
        musicValueText.text = $"{musicSlider.value + 80}";
        sensitivityValueText.text = $"{sensitivitySlider.value}";
    }

    //Unity docs warns us about setting values of audio mixer in Awake() and recommends setting it in Start()
    private void Start()
    {
        audioMixer.SetFloat("Master", masterAudioSlider.value);
        audioMixer.SetFloat("SFX", sfxSlider.value);
        audioMixer.SetFloat("Music", musicSlider.value);
    }

    //TODO: BUTTON SOUNDS

    protected virtual void OnContinueButton() { }

    private void OnStartButton()
    {
        SceneManager.LoadScene("AgahScene 1");
    }

    private void OnSettingButton()
    {
        mainScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    private void OnCreditsButton()
    {
        mainScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    private void BackToMainScreen()
    {
        mainScreen.SetActive(true);
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    private void ChangeMasterVolume(float newValue)
    {
        PlayerPrefs.SetFloat("MasterVolume", newValue);
        audioMixer.SetFloat("Master", newValue);
        masterAudioValueText.text = $"{newValue + 80}";
    }

    private void ChangeSfxVolume(float newValue)
    {
        PlayerPrefs.SetFloat("SFXVolume", newValue);
        audioMixer.SetFloat("SFX", newValue);
        sfxValueText.text = $"{newValue + 80}";
    }

    private void ChangeMusicVolume(float newValue)
    {
        PlayerPrefs.SetFloat("MusicVolume", newValue);
        audioMixer.SetFloat("Music", newValue);
        musicValueText.text = $"{newValue + 80}";
    }

    private void ChangeSensitivity(float newValue)
    {
        PlayerPrefs.SetFloat("Sensitivity", newValue / 10);
        PlayerInputManager.sensitivity = newValue / 10;
        sensitivityValueText.text = $"{newValue}";
    }
}
