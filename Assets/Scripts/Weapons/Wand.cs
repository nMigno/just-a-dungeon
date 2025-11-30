using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Wand : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab;

    private Transform tipTransform;
    private float lastShotTime = -Mathf.Infinity;

    private bool shooting = false;
    private Vector2 shootInput;

    void Start()
    {
        tipTransform = transform.Find("Wand Tip");
    }

    void Update()
    {
        TryShooting();
    }

    void TryShooting()
    {
        if (!shooting || Time.time < lastShotTime + GameManager.Instance.playerStats.ShootCooldown) return;

        // 1. We move the wand

        Vector2 shootDirection = shootInput.normalized;
        // 2. We shoot if CD is up

        if (Math.Abs(shootDirection.y) >= Math.Abs(shootDirection.x))
        {
            shootDirection.x = 0;
            shootDirection.y = Mathf.Sign(shootDirection.y);
        }
        else
        {
            shootDirection.y = 0;
            shootDirection.x = Mathf.Sign(shootDirection.x);
        }

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        lastShotTime = Time.time;

        GameObject bullet = Instantiate(bulletPrefab, tipTransform.position, Quaternion.identity);
        bullet.transform.localScale = GameManager.Instance.playerStats.BulletSize;

        if (bullet.TryGetComponent(out Rigidbody2D rbBullet))
        {
            rbBullet.linearVelocity = shootDirection * GameManager.Instance.playerStats.ShootSpeed;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            shooting = false;
            return;
        }
        if (context.performed)
        {
            shooting = true;
            shootInput = context.ReadValue<Vector2>();
        }
    }
}
