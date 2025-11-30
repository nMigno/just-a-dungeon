using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHpManager))]
[RequireComponent(typeof(PlayerController))]
public class MageHitVFX : MonoBehaviour
{
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string fireballTag = "Boss Fireball";

    [Header("Animation")]
    [SerializeField] private string isImmune = "Is Immune";
    [SerializeField] private string isHit = "Is Hit";
    [SerializeField] private string isDeadTrigger = "Is Dead";
    [SerializeField] private string isRunningBool = "Running";

    [Header("Settings")]
    [SerializeField] private float hitImmunityDuration = 1.0f;

    private Animator animator;
    private PlayerHpManager healthManager;
    private PlayerController playerController;

    private bool hitImmunity = false;
    private bool dashImmunity = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        healthManager = GetComponent<PlayerHpManager>();
        playerController = GetComponent<PlayerController>();

        healthManager.OnDeath.AddListener(HandleDeath);
    }

    void OnDestroy()
    {
        healthManager.OnDeath.RemoveListener(HandleDeath);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead || hitImmunity || dashImmunity) return;
        if (!(collision.CompareTag(enemyTag) || collision.CompareTag(fireballTag))) return;

        StartCoroutine(HitImmunity());

        healthManager.UpdateCurrentHealth(HealthOperation.Dec);
        animator.SetBool(isHit, true);
    }
    void UpdateAnimatorState()
    {
        bool finalImmunityState = hitImmunity || dashImmunity;
        animator.SetBool(isImmune, finalImmunityState);
    }

    IEnumerator HitImmunity()
    {
        hitImmunity = true;
        UpdateAnimatorState();

        yield return new WaitForSeconds(hitImmunityDuration);

        hitImmunity = false;
        animator.SetBool(isHit, false);
        UpdateAnimatorState();
    }

    public void SetDashImmunity(bool state)
    {
        dashImmunity = state;
        UpdateAnimatorState();
    }

    public void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        hitImmunity = false;
        dashImmunity = false;

        animator.SetBool(isHit, false);
        animator.SetBool(isImmune, false);
        animator.SetBool(isRunningBool, false);

        animator.SetTrigger(isDeadTrigger);
        playerController.OnDead();
    }
}
