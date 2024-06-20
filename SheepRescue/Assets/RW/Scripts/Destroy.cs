/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script manages the destruction of the haybales once they reach the end of the map.

using UnityEngine;

public class Destroy : MonoBehaviour
{
    public string tagFilter;


    // Destroys the haybales if they reach the end zone (so they don't continue forever creating too many game objects)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagFilter))
        {
            Destroy(gameObject);
        }
    }
}
