using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameInputMap { UI, Gameplay }

// Navigation System -- input system -- llevar track de la navegación del previo botón activo -- botón de salida a la derecha
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Indexes")]
    [SerializeField] private int splashScreenIndex = 1;
    [SerializeField] private int mainMenuIndex = 2;
    [SerializeField] private int creditsIndex = 3;
    [SerializeField] private int firstLevelIndex = 4;
    [Tooltip("If the index is lastLevelIndex, next scene goes to credits")]
    [SerializeField] private int lastLevelIndex = 23;
    [Tooltip("DEV_ONLY. If set, we jump to that level")]
    [SerializeField] private int devInitialLevel = -1;

    [Header("Input Action Maps")]
    [SerializeField] private string uiActionMap = "UI";
    [SerializeField] private string gameplayActionMap = "Gameplay";
    [SerializeField] private string splashActionMap = "Splash";

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("SoundSettings")]
    public float currentMusicVolume = 10f;
    public float currentSFXVolume = 10f;

    [Header("UI Sound Effects")]
    [SerializeField] private AudioClip uiConfirmClip;
    [SerializeField] private AudioClip uiCancelClip;
    [SerializeField] private AudioSource uiAudioSource;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.95f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.05f;

    private int currentSceneIndex;
    public PlayerStats playerStats;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();

        if (devInitialLevel == -1)
        {
            ToggleSplashKeymap(true);
            SceneManager.LoadSceneAsync(splashScreenIndex, LoadSceneMode.Additive);
            currentSceneIndex = splashScreenIndex;
        }
        else
        {
            ActivateActionMap(GameInputMap.UI);
            DEV_LoadLevel();
        }
    }

    void DEV_LoadLevel()
    {
        if (devInitialLevel == -1) return;

        int targetSceneIndex = firstLevelIndex + devInitialLevel - 1;
        SceneManager.LoadSceneAsync(targetSceneIndex, LoadSceneMode.Additive);
        currentSceneIndex = targetSceneIndex;
    }

    void ToggleSplashKeymap(bool activate)
    {
        if (activate)
        {
            InputSystem.actions.FindActionMap(splashActionMap).Enable();
            InputSystem.actions.FindActionMap(uiActionMap).Disable();
            InputSystem.actions.FindActionMap(gameplayActionMap).Disable();
        }
        else
        {
            InputSystem.actions.FindActionMap(splashActionMap).Disable();
            InputSystem.actions.FindActionMap(uiActionMap).Enable();
            InputSystem.actions.FindActionMap(gameplayActionMap).Disable();
        }
    }

    public void PlayUIConfirmSound()
    {
        uiAudioSource.pitch = Random.Range(minPitch, maxPitch);
        uiAudioSource.PlayOneShot(uiConfirmClip);
    }

    public void PlayUICancelSound()
    {
        uiAudioSource.pitch = Random.Range(minPitch, maxPitch);
        uiAudioSource.PlayOneShot(uiCancelClip);
    }

    public void ActivateActionMap(GameInputMap actionMapName)
    {
        switch (actionMapName)
        {
            case GameInputMap.UI:
                InputSystem.actions.FindActionMap(gameplayActionMap).Disable();
                InputSystem.actions.FindActionMap(uiActionMap).Enable();
                break;
            case GameInputMap.Gameplay:
                InputSystem.actions.FindActionMap(uiActionMap).Disable();
                InputSystem.actions.FindActionMap(gameplayActionMap).Enable();
                break;
        }
    }
    public void LoadMainMenu()
    {
        playerStats.ResetStats();
        ToggleSplashKeymap(false);
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        SceneManager.LoadSceneAsync(mainMenuIndex, LoadSceneMode.Additive);
        currentSceneIndex = mainMenuIndex;
    }

    public void StartGame()
    {
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        SceneManager.LoadSceneAsync(firstLevelIndex, LoadSceneMode.Additive);
        currentSceneIndex = firstLevelIndex;
        Debug.Log($"Current Index = {currentSceneIndex}");
    }

    public void LoadCredits()
    {
        SceneManager.UnloadSceneAsync(currentSceneIndex);
        SceneManager.LoadSceneAsync(creditsIndex, LoadSceneMode.Additive);
        InputSystem.actions.FindActionMap(uiActionMap).Enable();
        currentSceneIndex = creditsIndex;
    }

    public void LoadNextLevel()
    {
        int nextIndex = currentSceneIndex + 1;

        if (nextIndex > lastLevelIndex)
        {
            Debug.Log("Game finished! Showing credits");

            LoadCredits();
        }
        else
        {
            Debug.Log($"Loading Level {nextIndex}");
            SceneManager.UnloadSceneAsync(currentSceneIndex);
            SceneManager.LoadSceneAsync(nextIndex, LoadSceneMode.Additive);
            currentSceneIndex = nextIndex;

            Debug.Log($"Current Index = {currentSceneIndex}");
        }
    }

    public int GetFirstLevelIndex()
    {
        return firstLevelIndex;
    }

    public int GetCurrentLevelIndex()
    {
        return currentSceneIndex;
    }

    public void QuitGame()
    {
        Debug.Log("Closing game...");
        Application.Quit();
    }
}
