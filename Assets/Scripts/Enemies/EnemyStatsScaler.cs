using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHpManager))]
public class EnemyStatsScaler : MonoBehaviour
{
    [Header("Speed scaling per level")]
    [SerializeField] private float speedBonusPerLevel = 0.1f;

    private NavMeshAgent agent;
    private EnemyHpManager hpManager;

    // EVERYTHING: This file could be part of the LevelManager file since its the level responsability
    // to know its number, difficulty and configure it all accordingly.
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        hpManager = GetComponent<EnemyHpManager>();

        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        if (currentLevel == 0) currentLevel = 1;

        // Speed scaling logic
        float baseSpeed = agent.speed;
        float speedBonus = 1.0f + ((currentLevel - 1) * speedBonusPerLevel);

        agent.speed = baseSpeed * speedBonus;

        // Enemy HP scaling
        int baseHealth = hpManager.GetMaxHealth();
        int healthBonus = 0;

        if (currentLevel >= 16)
        {
            healthBonus = 3;
        }
        else if (currentLevel >= 11)
        {
            healthBonus = 2;
        }
        else if (currentLevel >= 6)
        {
            healthBonus = 1;
        }

        int finalHealth = baseHealth + healthBonus;
        hpManager.SetStartingHealth(finalHealth);
    }
}
