using UnityEngine;

public class PlayerPrefsInitialization : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("isGameOpened") == 0)
        {
            PlayerPrefs.SetInt("isGameOpened", 1);
            PlayerPrefs.SetFloat("Sensitivity", 0.5f);
            PlayerPrefs.SetFloat("MasterVolume", 1f);
            PlayerPrefs.SetFloat("SFXVolume", 1f);
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
    }
}