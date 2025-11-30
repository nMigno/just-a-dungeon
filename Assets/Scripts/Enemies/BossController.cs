using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BossStats))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyHpManager))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BossController : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new(1f);
    [SerializeField] private string mouthNode = "Mouth";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string bossPortalTag = "Boss Portal";
    [SerializeField] private GameObject fireballPrefab;

    [Header("Visual effects")]
    [SerializeField] private GameObject deathVfxPrefab;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip fireballClip;
    [SerializeField] private AudioClip fireBreathClip;
    [SerializeField] private AudioClip earthquakeClip;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Sound Variation")]
    [SerializeField, Range(0.1f, 3f)] private float minPitch = 0.9f;
    [SerializeField, Range(0.1f, 3f)] private float maxPitch = 1.1f;

    private Transform mouth;
    private Transform playerTransform;
    private BossStats stats;
    private Rigidbody2D body;
    private Transform[] teleporters;
    private EnemyHpManager bossHpManager;
    private AudioSource audioSource;
    private Animator animator;

    private int portalIndex = 1;
    private bool isDying = false;

    private Coroutine throwFireballCoroutine;
    private Coroutine teletransportCoroutine;

    void Awake()
    {
        stats = GetComponent<BossStats>();
        body = GetComponent<Rigidbody2D>();
        bossHpManager = GetComponent<EnemyHpManager>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        bossHpManager.onDeath.AddListener(StartDeathSequence);
        mouth = transform.Find(mouthNode);
        playerTransform = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Transform>();
        teleporters = GameObject
            .FindGameObjectsWithTag(bossPortalTag)
            .Select(bossPortal => bossPortal.transform)
            .ToArray();
    }

    void Start()
    {
        portalIndex = CalculateShorterTP();

        throwFireballCoroutine = StartCoroutine(ThrowFireball());
        teletransportCoroutine = StartCoroutine(Teletransport());
    }

    void OnDisable()
    {
        StopCoroutine(throwFireballCoroutine);
        StopCoroutine(teletransportCoroutine);
    }

    void OnDestroy()
    {
        StopCoroutine(throwFireballCoroutine);
        StopCoroutine(teletransportCoroutine);
    }

    IEnumerator ThrowFireball()
    {
        while (true)
        {
            yield return new WaitForSeconds(stats.FireballCD);
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(fireballClip);
            GameObject fireball = Instantiate(fireballPrefab, mouth.position, Quaternion.identity);

            fireball.transform.SetParent(mouth);

            var heading = playerTransform.position - mouth.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            var fireballBody = fireball.GetComponent<Rigidbody2D>();

            fireballBody.linearVelocity = direction * stats.FireballSpeed;

        }
    }

    public void ReceiveDamage(int damage)
    {
        if (isDying) return;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(hitClip);
        bossHpManager.ReceiveDamage(damage);
        animator.SetTrigger("Hit");
    }

    void StartDeathSequence()
    {
        if (isDying) return;

        StopCoroutine(throwFireballCoroutine);
        StopCoroutine(teletransportCoroutine);

        Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
        body.linearVelocity = Vector2.zero;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(deathClip);
        isDying = true;

        animator.SetTrigger("Die");
        DestroyEnemy();
    }

    IEnumerator Teletransport()
    {
        while (true)
        {
            yield return _waitForSeconds1;

            int nextIndex = CalculateShorterTP();

            if (nextIndex != portalIndex)
            {
                portalIndex = nextIndex;
                audioSource.PlayOneShot(teleportClip);
                body.position = teleporters[portalIndex].position;

                stats.TeleportPenalization();

                yield return new WaitForSeconds(stats.TeletransportCD);
            }
        }
    }

    int CalculateShorterTP()
    {

        int nextIndex = portalIndex;
        Vector3 playerPosition = playerTransform.position;
        float nextDistance = (playerPosition - teleporters[nextIndex].position).magnitude;

        for (int i = 0; i < teleporters.Length; i++)
        {
            if (nextIndex == i) continue;

            float distance = (playerPosition - teleporters[i].position).magnitude;

            if (distance < nextDistance)
            {
                nextIndex = i;
                nextDistance = distance;
            }
        }

        return nextIndex;
    }

    public void DestroyEnemy()
    {
        LevelManager.Instance.OnEnemyDefeated.Invoke();
        float delay = deathClip.length;
        Destroy(gameObject, delay);
    }
}
