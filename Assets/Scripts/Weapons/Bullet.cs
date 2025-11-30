using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifetime = 2.0f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string terrainTag = "Wall";
    [SerializeField] private string fireballTag = "Boss Fireball";
    [SerializeField] private string bossTag = "Boss";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string bulletTag = "Bullet";

    private readonly int bulletDamage = 1;
    private Rigidbody2D body;

    void Start()
    {
        Destroy(gameObject, lifetime);
        body = GetComponent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag) || collision.CompareTag(bulletTag))
        {
            return;
        }

        if (collision.CompareTag(terrainTag) || collision.CompareTag(fireballTag))
        {
            Destroy(gameObject);

            return;
        }

        if (collision.CompareTag(bossTag))
        {
            BossController boss = collision.GetComponentInParent<BossController>();
            boss.ReceiveDamage(bulletDamage);
            Destroy(gameObject);
            return;
        }

        if (!collision.CompareTag(enemyTag)) return;

        Enemy enemy = collision.GetComponentInParent<Enemy>();

        enemy.ReceiveDamage(bulletDamage);

        Destroy(gameObject);
    }
}
