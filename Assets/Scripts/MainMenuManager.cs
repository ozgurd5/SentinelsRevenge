using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Assign - Screens")]
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject creditsScreen;

    [Header("Assign - Main Screen Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Assign - Back Buttons")]
    [SerializeField] private Button backFromSettingsButton;
    [SerializeField] private Button backFromCreditsButton;

    [Header("Assign - Settings Sliders")]
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Assign - Settings Texts")]
    [SerializeField] private TextMeshProUGUI masterAudioValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sensitivityValueText;

    [Header("Assign - Audio Mixer")] [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartButton);
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

    private void OnStartButton()
    {
        SceneManager.LoadScene("OzgurScene");
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
