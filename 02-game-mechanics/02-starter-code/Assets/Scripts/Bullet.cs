using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float speed;
    private Transform target;

    public void SetBulletProperties(float bulletSpeed, float bulletDamage)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
    }

    void Update()
    {
        if (target != null)
        {
            // Move the bullet towards target
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else
        {
            // Else destroy bullet
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponentInChildren<EnemyHealth>(); // Gets reference to enemy health component
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage); // Calls TakeDamage function on EnemyHealth script
            }

            Destroy(gameObject); // Destroy bullet after hitting the enemy
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
