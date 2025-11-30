using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(AudioSource))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private int finalLevelIndex = 23;
    [Header("Tags")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string bossTag = "Boss";

    [Header("HUD reference")]
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private TextMeshProUGUI enemyCounterText;

    [SerializeField] private GameObject screenLevelComplete;
    [SerializeField] private GameObject warningMenuPanel;
    [SerializeField] private GameObject screenGameOver;
    [SerializeField] private GameObject screenGameComplete;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseMenuPanel;

    [Header("Card Selection")]
    [SerializeField] private CardScreenController cardScreenController;

    [Header("Level Sounds")]
    [SerializeField] private AudioClip levelCompleteClip;
    [SerializeField] private AudioClip cardSelectClip;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.95f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.05f;

    private AudioSource sceneAudioSource;

    public UnityEvent OnEnemyDefeated;

    private int remainingEnemies;

    void Awake()
    {
        Instance = this;
        sceneAudioSource = GetComponent<AudioSource>();
    }

    public void PauseGame()
    {
        Debug.Log("Game Paused");

        hudCanvas.SetActive(false);
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        GameManager.Instance.ActivateActionMap(GameInputMap.UI);
    }

    void Start()
    {
        OnEnemyDefeated.AddListener(EnemyDefeated);

        screenLevelComplete.SetActive(false);
        screenGameOver.SetActive(false);
        hudCanvas.SetActive(false);
        cardScreenController.ShowCardSelection();
    }

    void OnDestroy()
    {
        OnEnemyDefeated.RemoveListener(EnemyDefeated);
    }

    void EnemyDefeated()
    {
        remainingEnemies--;

        UpdateHUD();

        if (remainingEnemies == 0)
        {
            CompleteLevel();
        }
    }

    void UpdateHUD()
    {
        enemyCounterText.text = remainingEnemies.ToString();
    }

    public void CompleteLevel()
    {
        sceneAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        sceneAudioSource.PlayOneShot(levelCompleteClip);

        GameManager.Instance.ActivateActionMap(GameInputMap.UI);
        warningMenuPanel.SetActive(false);

        int currentLevelIndex = GameManager.Instance.GetCurrentLevelIndex();
        if (currentLevelIndex == finalLevelIndex)
        {
            screenGameComplete.SetActive(true);
        }
        else
        {
            screenLevelComplete.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void WarningBackToMenu()
    {
        GameManager.Instance.PlayUIConfirmSound();
        screenLevelComplete.SetActive(false);
        warningMenuPanel.SetActive(true);
    }

    public void StartLevel()
    {
        hudCanvas.SetActive(true);
        Time.timeScale = 1f;
        GameManager.Instance.ActivateActionMap(GameInputMap.Gameplay);
        int enemyCount = GameObject.FindGameObjectsWithTag(enemyTag).Length;
        int bossCount = GameObject.FindGameObjectsWithTag(bossTag).Length;
        remainingEnemies = enemyCount + bossCount;

        UpdateHUD();
    }

    public void GoToNextLevel()
    {
        GameManager.Instance.PlayUIConfirmSound();
        GameManager.Instance.LoadNextLevel();
    }

    public void ActivateGameOver()
    {
        screenGameOver.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.ActivateActionMap(GameInputMap.UI);
    }

    public void CardSelect(CardType cardType)
    {
        sceneAudioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        sceneAudioSource.PlayOneShot(cardSelectClip);
        GameManager.Instance.playerStats.ApplyPowerUp(cardType);
        cardScreenController.HideCardSelection();

        StartLevel();
    }

    public void GoBackToMenu()
    {
        GameManager.Instance.PlayUIConfirmSound();
        GameManager.Instance.LoadMainMenu();
    }
}
