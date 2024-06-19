using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    private float originalXScale;
    public GameObject enemyParent;


    private void Start()
    {
        // The health bar takes note of its initial x scale, so that it can
        // rescale itself relative to that initial scale.
        originalXScale = gameObject.transform.localScale.x;
    }

    private void Update()
    {
        // Get the current scale of the gameObject.
        Vector3 newScale = gameObject.transform.localScale;

        // Calculate the health percentage.
        float healthPercentage = currentHealth / maxHealth;

        // Update the x scale based on the health percentage.
        newScale.x = originalXScale * healthPercentage;

        // Apply the new scale to the gameObject.
        gameObject.transform.localScale = newScale;
    }

    public void TakeDamage(float damage)
    {
        // Reduce current health by the damage amount
        currentHealth -= damage;

        // Check if current health is <= 0
        if (currentHealth <= 0)
        {
            // If health is 0 or less, destroy the enemy
            Destroy(enemyParent);

            // Award gold to the player (You need to implement GameManager for this)
            GameManager.Instance.AddGold(50); // Example: Add 10 gold to player's total
        }
    }
}
