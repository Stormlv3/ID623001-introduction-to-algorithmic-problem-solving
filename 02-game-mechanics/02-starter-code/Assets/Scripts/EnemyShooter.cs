using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    private MonsterData monsterData; // Reference to MonsterData script
    private float lastShotTime;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private void Start()
    {
        monsterData = GetComponent<MonsterData>(); // Get reference to MonsterData script
    }

    private void Update()
    {
        if (Time.time - lastShotTime >= GetCurrentShootCooldown())
        {
            if (enemiesInRange.Count > 0)
            {
                // Shoot at the first available enemy
                Shoot(enemiesInRange[0]);
                SFXManager.Instance.PlayShootSFX();
            }
        }
    }

    private float GetCurrentShootCooldown()
    {
        if (monsterData == null)
        {
            return 0f;
        }

        // Get the current level's shoot cooldown
        MonsterData.MonsterLevel currentLevel = monsterData.CurrentLevel;
        return currentLevel.shootCooldown;
    }

    public void Shoot(GameObject target)
    {
        if (monsterData == null)
        {
            return;
        }

        // Get the current level's bullet properties
        MonsterData.MonsterLevel currentLevel = monsterData.CurrentLevel;
        float bulletSpeed = currentLevel.bulletSpeed;

        // Calculate direction and set rotation of the EnemyShooter
        Vector3 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f; // Adjust by 180 degrees
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Instantiate a bullet and set its properties
        GameObject bullet = Instantiate(currentLevel.bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.SetTarget(target.transform);
            bulletComponent.SetBulletProperties(bulletSpeed, currentLevel.bulletDamage);
        }

        // Update lastShotTime
        lastShotTime = Time.time;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Remove(other.gameObject);
            }
        }
    }
}
