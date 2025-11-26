using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class SplashScreen : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionReference confirmAction;

    [Header("Sound")]
    [SerializeField] private AudioClip confirmClip;
    private AudioSource audioSource;
    private bool isLoading = false;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.95f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.05f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        confirmAction.action.started += OnSubmit;
    }

    void OnDisable()
    {
        confirmAction.action.started -= OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext context)
    {
        if (isLoading) return;
        isLoading = true;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(confirmClip);
        GameManager.Instance.LoadMainMenu();
    }
}
