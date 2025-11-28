using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyHpManager))]
public class EnemyHpManager : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    private int currentHealth;

    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void ReceiveDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log(gameObject.name + " received " + damage + " of damage. Remaining life: " + currentHealth);

        if (currentHealth == 0)
        {
            Debug.Log(gameObject.name + " died.");

            onDeath.Invoke();
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetStartingHealth(int newMaxHealth)
    {
        currentHealth = newMaxHealth;
    }
}
