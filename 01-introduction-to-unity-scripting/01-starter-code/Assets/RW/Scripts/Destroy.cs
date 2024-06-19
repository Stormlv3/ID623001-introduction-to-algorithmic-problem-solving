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
