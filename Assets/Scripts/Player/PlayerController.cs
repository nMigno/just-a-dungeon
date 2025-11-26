using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum HandleToggle
{
    ON, OFF
}
enum HandleAction
{
    MOVE, ATTACK, DASH, PAUSE
}

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerHpManager))]
[RequireComponent(typeof(MageHitVFX))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Animation States")]
    [SerializeField] private string running = "Running";
    [SerializeField] private string facing = "CurrentInput";
    [SerializeField] private string lastFaced = "LastInput";

    [Header("Inputs")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference attack;
    [SerializeField] private InputActionReference dash;
    [SerializeField] private InputActionReference pause;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip dashClip;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.9f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.1f;

    private Rigidbody2D body;
    private Vector2 playerInput;
    private Animator animator;
    private MageHitVFX mageHitVFX;
    private Wand weapon;
    private AudioSource audioSource;
    private PlayerStats stats;

    private bool dashing = false;
    private bool attacking = false;
    private bool isSlowed = false;

    private Coroutine cancelAttackCoroutineId;
    private Coroutine resetSpeedCoroutineId;

    private Vector2 latestLinearVelocity;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        mageHitVFX = GetComponent<MageHitVFX>();
        weapon = GetComponentInChildren<Wand>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        stats = GameManager.Instance.playerStats;
    }

    void FixedUpdate()
    {
        if (dashing || !enabled) return;

        Vector2 currentVelocity = playerInput;

        if (isSlowed)
        {
            currentVelocity *= stats.MovePenalization;
        }

        body.linearVelocity = stats.MoveSpeed * currentVelocity;
        latestLinearVelocity = body.linearVelocity;
    }

    void OnEnable()
    {
        HandleActionEvents(HandleAction.ATTACK, HandleToggle.ON);
        HandleActionEvents(HandleAction.MOVE, HandleToggle.ON);
        HandleActionEvents(HandleAction.DASH, HandleToggle.ON);
        HandleActionEvents(HandleAction.PAUSE, HandleToggle.ON);
    }

    void OnDisable()
    {
        HandleActionEvents(HandleAction.ATTACK, HandleToggle.OFF);
        HandleActionEvents(HandleAction.MOVE, HandleToggle.OFF);
        HandleActionEvents(HandleAction.DASH, HandleToggle.OFF);
        HandleActionEvents(HandleAction.PAUSE, HandleToggle.OFF);
    }

    void HandleActionEvents(HandleAction action, HandleToggle handle)
    {
        switch (action)
        {
            case HandleAction.ATTACK:
                switch (handle)
                {
                    case HandleToggle.ON:
                        attack.action.performed += weapon.OnShoot;
                        attack.action.canceled += weapon.OnShoot;
                        attack.action.performed += OnShoot;
                        attack.action.canceled += OnShoot;
                        break;
                    case HandleToggle.OFF:
                        attack.action.performed -= weapon.OnShoot;
                        attack.action.canceled -= weapon.OnShoot;
                        attack.action.performed -= OnShoot;
                        attack.action.canceled -= OnShoot;
                        break;
                }
                break;
            case HandleAction.MOVE:
                switch (handle)
                {
                    case HandleToggle.ON:
                        move.action.performed += OnMove;
                        move.action.canceled += OnCancelMove;
                        break;
                    case HandleToggle.OFF:
                        move.action.performed -= OnMove;
                        move.action.canceled -= OnCancelMove;
                        break;
                }
                break;
            case HandleAction.DASH:
                switch (handle)
                {
                    case HandleToggle.ON:
                        dash.action.started += OnDash;
                        break;
                    case HandleToggle.OFF:
                        dash.action.started -= OnDash;
                        break;
                }
                break;
            case HandleAction.PAUSE:
                switch (handle)
                {
                    case HandleToggle.ON:
                        pause.action.performed += OnPause;
                        break;
                    case HandleToggle.OFF:
                        pause.action.performed -= OnPause;
                        break;
                }
                break;
        }
    }

    void OnCancelMove(InputAction.CallbackContext context)
    {
        playerInput = Vector2.zero;

        if (dashing) return;

        body.linearVelocity = Vector2.zero;
        animator.SetBool(running, false);

        if (playerInput.x != 0)
        {
            animator.SetFloat(lastFaced, playerInput.x);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        playerInput = context.ReadValue<Vector2>();

        if (dashing) return;

        animator.SetBool(running, true);
        animator.SetFloat(facing, playerInput.x);

        /*Vector2 currentVelocity = playerInput;

        if (isSlowed) currentVelocity *= stats.MovePenalization;

        body.linearVelocity = stats.MoveSpeed * currentVelocity;
        latestLinearVelocity = body.linearVelocity;*/
    }

    void OnDash(InputAction.CallbackContext context)
    {
        if (!attacking && (body.linearVelocity.x != 0 || body.linearVelocity.y != 0))
        {
            HandleActionEvents(HandleAction.DASH, HandleToggle.OFF);

            StartCoroutine(DashCoroutine());
        }
    }

    void OnShoot(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            cancelAttackCoroutineId = StartCoroutine(CancelAttackCoroutine());
            resetSpeedCoroutineId = StartCoroutine(ResetSpeedCoroutine());

            return;
        }

        if (context.performed)
        {
            if (cancelAttackCoroutineId != null) StopCoroutine(cancelAttackCoroutineId);
            if (resetSpeedCoroutineId != null) StopCoroutine(resetSpeedCoroutineId);

            if (!isSlowed)
            {
                body.linearVelocity *= stats.MovePenalization;
            }

            attacking = true;
            isSlowed = true;

            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(shootClip);
        }
    }
    void OnPause(InputAction.CallbackContext context)
    {
        LevelManager.Instance.PauseGame();
    }

    IEnumerator ResetSpeedCoroutine()
    {
        yield return new WaitForSeconds(stats.MovePenalizationDuration);

        isSlowed = false;

        if (playerInput != Vector2.zero && !dashing)
        {
            body.linearVelocity = stats.MoveSpeed * playerInput;
            latestLinearVelocity = body.linearVelocity;
        }
    }

    IEnumerator CancelAttackCoroutine()
    {
        yield return new WaitForSeconds(stats.AttackPenalization);

        attacking = false;
    }

    IEnumerator DashCoroutine()
    {
        dashing = true;
        mageHitVFX.StartImmunity();
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(dashClip);

        body.linearVelocity = body.linearVelocity.normalized * stats.DashVelocity;

        yield return new WaitForSeconds(stats.DashDuration);

        dashing = false;

        body.linearVelocity = stats.MoveSpeed * playerInput;

        mageHitVFX.EndImmunity();

        yield return new WaitForSeconds(stats.DashCooldown);

        HandleActionEvents(HandleAction.DASH, HandleToggle.ON);
    }

    public void OnDead()
    {
        enabled = false;
        body.linearVelocity = Vector2.zero;
    }
}
