using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaseController : MonoBehaviour
{
    [Header("Group settings")]
    public float detectionRadius = 10f;
    public float loseRadius = 15f;

    [Header("Movement")]
    public float wanderRadius = 5f;
    public float wanderTimer = 4f;

    private Transform player;
    private NavMeshAgent agent;
    private Vector3 startPosition;
    private float timer;
    private bool isChasing = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        startPosition = transform.position;
        timer = wanderTimer;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;
    }

    void Update()
    {
        if (player == null) return;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        animator.speed = 1;
        animator.SetFloat("Facing", agent.velocity.x);

        if (isChasing)
        {
            LookAtPlayer();

            if (distanceToPlayer > loseRadius)
            {
                StopChasing();
            }
            else
            {
                Chase();
            }
        }
        else
        {
            if (distanceToPlayer < detectionRadius)
            {
                StartChasing();
            }
            else
            {
                Patrol();
            }
        }
    }

    void StartChasing()
    {
        isChasing = true;
    }

    void StopChasing()
    {
        isChasing = false;

        timer = wanderTimer;
        if(agent.isOnNavMesh) agent.SetDestination(startPosition);
    }

    void Chase()
    {
        if (!agent.isOnNavMesh) return;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 2.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            Vector3 newPos = RandomNavSphere(startPosition, wanderRadius, -1);
            agent.SetDestination(newPos);
        }
    }

    void LookAtPlayer()
    {
        float directionX = player.position.x - transform.position.x;

        if (directionX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (directionX > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, wanderRadius);
    }
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector2 randomPoint2D = Random.insideUnitCircle * dist;
        Vector3 randomDirection = new Vector3(randomPoint2D.x, randomPoint2D.y, 0) + origin;
        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask))
        {
            return navHit.position;
        }
        return origin;
    }

}
