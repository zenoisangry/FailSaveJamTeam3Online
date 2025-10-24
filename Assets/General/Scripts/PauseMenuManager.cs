using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseDisplay;
    public GameObject settingsDisplay;
    public Slider sensitivitySlider;
    public Slider musicSlider;
    public Slider SFXSlider;
    public Slider volumeSlider;
    public Button returnButton;

    [Header("Audio")]
    public AudioMixer masterMixer;

    [Header("Input")]
    public InputActionAsset inputActions;

    [Header("References")]
    public PlayerMovement playerMovement;

    private InputAction pauseActionPlayer;
    private InputAction pauseActionUI;

    private bool isPauseMenuActive = false;
    private bool isSettingsActive = false;

    private void Awake()
    {
        pauseActionPlayer = InputSystem.actions.FindAction("Player/Pause");
        pauseActionUI = InputSystem.actions.FindAction("UI/Pause");

        LoadSettings();

        pauseDisplay.SetActive(false);
        settingsDisplay.SetActive(false);

        if (returnButton != null)
            returnButton.onClick.AddListener(ClosePauseMenu);
    }

    private void OnEnable()
    {
        inputActions.FindActionMap("UI").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("UI").Disable();
    }

    private void Update()
    {
        HandlePauseInput();
    }

    // ---------------------------------------------------------
    // MENU PAUSA
    // ---------------------------------------------------------

    private void HandlePauseInput()
    {
        if (pauseActionPlayer.WasPressedThisFrame() && !isPauseMenuActive)
        {
            OpenPauseMenu();
        }
        else if (pauseActionUI.WasPressedThisFrame() && isPauseMenuActive && !isSettingsActive)
        {
            ClosePauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        isPauseMenuActive = true;
        pauseDisplay.SetActive(true);
        settingsDisplay.SetActive(false);

        inputActions.FindActionMap("Player").Disable();
        inputActions.FindActionMap("UI").Enable();

        Time.timeScale = 0f;
    }

    public void ClosePauseMenu()
    {
        isPauseMenuActive = false;
        pauseDisplay.SetActive(false);
        settingsDisplay.SetActive(false);

        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("UI").Disable();

        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        isSettingsActive = true;
        settingsDisplay.SetActive(true);
    }

    public void CloseSettings()
    {
        isSettingsActive = false;
        settingsDisplay.SetActive(false);
        SaveSettings();
    }

    // ---------------------------------------------------------
    // IMPOSTAZIONI
    // ---------------------------------------------------------

    public void SetSensitivity()
    {
        if (playerMovement != null && sensitivitySlider != null)
        {
            float value = sensitivitySlider.value;
            playerMovement.SetMouseSensitivity(value);
            playerMovement.SetControllerSensitivity(value);
        }
    }

    public void SetVolume()
    {
        float music = musicSlider.value;
        masterMixer.SetFloat("BackgroundMusic", Mathf.Log10(music) * 20);

        float volume = volumeSlider.value;
        masterMixer.SetFloat("VolumeMusic", Mathf.Log10(volume) * 20);

        float sfx = SFXSlider.value;
        masterMixer.SetFloat("SoundEffects", Mathf.Log10(sfx) * 20);
    }

    // ---------------------------------------------------------
    // SALVATAGGIO E CARICAMENTO
    // ---------------------------------------------------------

    private void SaveSettings()
    {
        if (sensitivitySlider != null)
            PlayerPrefs.SetFloat("Sensitivity", sensitivitySlider.value);

        if (musicSlider != null)
            PlayerPrefs.SetFloat("BackgroundMusic", musicSlider.value);

        if (volumeSlider != null)
            PlayerPrefs.SetFloat("VolumeMusic", musicSlider.value);

        if (SFXSlider != null)
            PlayerPrefs.SetFloat("SoundEffects", musicSlider.value);

        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        float music = PlayerPrefs.GetFloat("BackgroundMusic", 1.0f);
        float volume = PlayerPrefs.GetFloat("VolumeMusic", 1.0f);
        float sfx = PlayerPrefs.GetFloat("SoundEffects", 1.0f);

        if (playerMovement != null)
        {
            playerMovement.SetMouseSensitivity(sensitivity);
            playerMovement.SetControllerSensitivity(sensitivity);
        }

        if (masterMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(music, 0.001f, 1f)) * 20f;
            masterMixer.SetFloat("BackgroundMusic", dB);

            float VdB = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f;
            masterMixer.SetFloat("VolumeMusic", VdB);

            float SdB = Mathf.Log10(Mathf.Clamp(sfx, 0.001f, 1f)) * 20f;
            masterMixer.SetFloat("SoundEffects", SdB);
        }

        if (sensitivitySlider != null)
            sensitivitySlider.value = sensitivity;

        if (musicSlider != null)
            musicSlider.value = music;

        if (volumeSlider != null)
            volumeSlider.value = volume;

        if (SFXSlider != null)
            SFXSlider.value = sfx;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}