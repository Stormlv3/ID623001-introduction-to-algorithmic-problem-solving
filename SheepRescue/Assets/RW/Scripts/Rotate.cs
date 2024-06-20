/// 
/// Author: Lucas Storm
/// May 2024
/// Bugs: None known at this time.
/// 
/// This script is used to rotate the windmill blades.

using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float RotationSpeedMin = 50f;
    public float RotationSpeedMax = 100f;
    private float rotationSpeed;

    void Start()
    {
        // Choose random rotate speed
        rotationSpeed = Random.Range(RotationSpeedMin, RotationSpeedMax);
    }
    void Update()
    {
        // Roate the windmil wheel around axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
