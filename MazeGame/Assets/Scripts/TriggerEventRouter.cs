/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This handles the triggers for win and loose states.


using UnityEngine;


public class TriggerEventRouter : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // If tag is monster then game over
        if (other.CompareTag("Monster"))
        {
            GameSetup.Instance.GameLose();
        }
        // Else if tag is treasure then game won
        else if (other.CompareTag("Treasure"))
        {
            GameSetup.Instance.GameWon();
        }
    }
}