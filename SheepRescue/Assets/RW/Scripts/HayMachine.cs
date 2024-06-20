/// 
/// Author: Lucas Storm
/// May 2024
/// Bugs: None known at this time.
/// 
/// This script handles the movement and shooting that the
/// hay machine does.

using UnityEngine;

public class HayMachine : MonoBehaviour
{
    public float movementSpeed = 15f;
    public float horizontalBoundary = 22;
    public GameObject hayBalePrefab;
    public Transform haySpawnpoint;
    public float shootInterval;
    private float shootTimer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PerformMovement(); // Move the hay machine
        HandleShooting(); // Handle shooting hay bales
    }

    void PerformMovement()
    {
        // Get input for horizontal movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Move the machine based on input and boundaries
        if (horizontalInput < 0 && transform.position.x > -horizontalBoundary)
        {
            transform.Translate(transform.right * -movementSpeed * Time.deltaTime);
        }
        else if (horizontalInput > 0 && transform.position.x < horizontalBoundary)
        {
            transform.Translate(transform.right * movementSpeed * Time.deltaTime);
        }
    }
    void HandleShooting()
    {
        // Update shoot timer
        shootTimer -= Time.deltaTime;

        // Shoot hay if timer is up and space key is pressed
        if (shootTimer <= 0 && Input.GetKey(KeyCode.Space))
        {
            shootTimer = shootInterval;
            ShootHay();
        }
    }

    private void ShootHay()
    {
        // Play shoot sound effect
        SFXManager.Instance.PlayShootSFX();

        // Instantiate a hay bale at the spawn point
        Instantiate(hayBalePrefab, haySpawnpoint.position, Quaternion.identity);
    }
}
