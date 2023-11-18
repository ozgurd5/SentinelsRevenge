using UnityEngine;

public class PauseMenuManager : MenuManager
{
    private PlayerStateData psd;
    private Canvas canvas;

    private PlayerStateData.PlayerMainState previousPlayerMainState;
    private bool isGamePaused;

    private void Awake()
    {
        psd = GameObject.Find("Player").GetComponent<PlayerStateData>();
        canvas = GetComponent<Canvas>();

        //Default settings
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        OnAwake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused) OnContinueButton();
            else PauseGame();
        }
    }

    //Naming make sense in parent class but not in here :(
    protected override void OnContinueButton()
    {
        isGamePaused = false;
        canvas.enabled = false;

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        psd.playerMainState = previousPlayerMainState;
        
        mainScreen.SetActive(true);
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);
    }

    private void PauseGame()
    {
        isGamePaused = true;
        canvas.enabled = true;

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        previousPlayerMainState = psd.playerMainState;
        psd.playerMainState = PlayerStateData.PlayerMainState.Paused;
    }
}
