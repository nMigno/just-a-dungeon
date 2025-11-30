using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float movePenalization = 0.5f;
    [SerializeField] private float movePenalizationDuration = 0.15f;

    [Header("Attack Stats")]
    [SerializeField] private float shootSpeed = 10.0f;
    [SerializeField] private float shootCooldown = 0.8f;
    [SerializeField] private Vector2 bulletSize = new(1f, 1f);
    [Tooltip("In seconds")]
    [SerializeField] private float attackPenalization = 0.05f;

    [Header("Dash Stats")]
    [SerializeField] private float dashCooldown = 2.5f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashVelocity = 20f;

    public float MoveSpeedBonus { get; private set; } = 0f;
    public float ShootCooldownBonus { get; private set; } = 0f;
    public float DashCooldownBonus { get; private set; } = 0f;
    public float BulletSizeBonus { get; private set; } = 0f;

    // Public stats
    public float MoveSpeed { get; private set; }
    public float MovePenalization { get; private set; }
    public float MovePenalizationDuration { get; private set; }

    public float ShootSpeed { get; private set; }
    public float ShootCooldown { get; private set; }
    public Vector2 BulletSize { get; private set; }
    public float AttackPenalization { get; private set; }

    public float DashCooldown { get; private set; }
    public float DashDuration { get; private set; }
    public float DashVelocity { get; private set; }

    void Start()
    {
        MovePenalization = movePenalization;
        ShootSpeed = shootSpeed;
        DashDuration = dashDuration;
        DashVelocity = dashVelocity;
        AttackPenalization = attackPenalization;
        MovePenalizationDuration = movePenalizationDuration;
                
        GetActualStats(GameManager.Instance.playerStats);
    }

    void RecalculateStats()
    {
        MoveSpeed = moveSpeed * (1f + MoveSpeedBonus);
        ShootCooldown = shootCooldown / (1f + ShootCooldownBonus);
        DashCooldown = dashCooldown / (1f + DashCooldownBonus);
        BulletSize = bulletSize * (1f + BulletSizeBonus);
    }

    public void GetActualStats(PlayerStats sourceStats)
    {
        MoveSpeedBonus = sourceStats.MoveSpeedBonus;
        ShootCooldownBonus = sourceStats.ShootCooldownBonus;
        DashCooldownBonus = sourceStats.DashCooldownBonus;
        BulletSizeBonus = sourceStats.BulletSizeBonus;

        RecalculateStats();
    }

    public void ApplyPowerUp(CardType selectedPowerUp)
    {
        switch (selectedPowerUp)
        {
            case CardType.Mage:
                ShootCooldownBonus += 0.10f;
                Debug.Log($"New ASPD Bonus: {ShootCooldownBonus}");                
                break;
            case CardType.Chariot:
                MoveSpeedBonus += 0.10f;
                Debug.Log($"New MSPD Bonus {MoveSpeedBonus}");
                break;
            case CardType.Wheel:
                DashCooldownBonus += 0.10f;
                Debug.Log($"New DashCD Bonus: {DashCooldownBonus}" );
                break;
            case CardType.Star:
                BulletSizeBonus += 0.05f;
                Debug.Log($"New bullet size bonus: {BulletSizeBonus}");
                break;
        }
        RecalculateStats();

        Debug.Log($"ASPD: {ShootCooldown}");
        Debug.Log($"MSPD: {MoveSpeed}");
        Debug.Log($"DashCD: {DashCooldown}");
        Debug.Log($"BulletSize: {BulletSize}");
    }

    public void ResetStats()
    {
        MoveSpeedBonus = 0f;
        ShootCooldownBonus = 0f;
        DashCooldownBonus = 0f;
        BulletSizeBonus = 0f;

        RecalculateStats();
    }

    public void SetDevStats(float moveBonus, float shootBonus, float dashBonus, float sizeBonus)
    {
        MoveSpeedBonus = moveBonus;
        ShootCooldownBonus = shootBonus;
        DashCooldownBonus = dashBonus;
        BulletSizeBonus = sizeBonus;

        RecalculateStats();

        Debug.LogWarning("DEV MODE: Custom stats applied");
        Debug.Log($"ASPD: {ShootCooldown} | MSPD: {MoveSpeed} | DashCD: {DashCooldown} | BulletSize: {BulletSize}");
    }
}
