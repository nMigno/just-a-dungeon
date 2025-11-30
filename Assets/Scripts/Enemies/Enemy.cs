using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyHpManager))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private string isHit = "Is Hit";

    [Header("Stats")]
    [SerializeField] private int enemyDamage;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.9f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.1f;

    private bool isDying = false;
    private EnemyHpManager hpManager;
    private Rigidbody2D body;
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer sprite;
    private Collider2D collider2d;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hpManager = GetComponent<EnemyHpManager>();
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        sprite = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();

        hpManager.onDeath.AddListener(StartDeathSequence);
    }

    void Update()
    {
        if (isDying) return;
        if (Mathf.Abs(agent.velocity.x) > 0.1f)
        {
            float direction = Mathf.Sign(agent.velocity.x);
            animator.SetFloat("Facing", direction);
        }
    }

    void OnDestroy()
    {
        hpManager.onDeath.RemoveListener(StartDeathSequence);
    }

    void StartDeathSequence()
    {
        if (isDying) return;

        isDying = true;
        // Everything below is for reproducing dead sound while mob is dying.
        agent.isStopped = true;
        body.linearVelocity = Vector2.zero;
        sprite.enabled = false;
        collider2d.enabled = false;
        agent.enabled = false;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(deathClip, 1.5f);

        DestroyEnemy();
    }

    public void ReceiveDamage(int damage)
    {
        if (isDying) return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.PlayOneShot(hitClip, 1.5f);
        hpManager.ReceiveDamage(damage);
        animator.SetTrigger("Hit");
    }

    public void EnableAI()
    {
        animator.SetBool(isHit, false);

        if (isDying) return;

        body.linearVelocity = Vector2.zero;
        agent.isStopped = false;
    }

    public void DestroyEnemy()
    {
        LevelManager.Instance.OnEnemyDefeated.Invoke();

        Destroy(gameObject, deathClip.length);
    }
}
