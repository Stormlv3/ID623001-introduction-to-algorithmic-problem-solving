/// 
/// Author: Lucas Storm
/// June 2024
/// Bugs: None known at this time.
///
/// This class handles the majority of the core gameplay
/// Such as keeping track of the sheep that have been saved
/// or dropped. Additionally it handles the game over state.

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Number tracking how many sheep we've saved
    [HideInInspector]
    public int sheepSaved;

    // Number tracking how many sheep have been dropped
    [HideInInspector]
    public int sheepDropped;

    // Number of sheep that can be dropped before game over
    public int sheepDroppedBeforeGameOver = 3;


    public SpawnSheep sheepSpawner;

    public void Awake()
    {
        Instance = this;
    }

    public void SaveSheep()
    {
        // Increase the number of sheep saved by 1
        sheepSaved++;
        // Runs the function in UIManager to add one to the score
        UIManager.Instance.IncreaseScore();
    }

    public void DroppedSheep()
    {
        // Record that we have dropped a sheep
        sheepDropped++;
        // Runs the function in UIManager to remove a life
        UIManager.Instance.LifeLost();

        // Call GameOver() if we have dropped too many sheep
        if (sheepDropped >= sheepDroppedBeforeGameOver)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // Destroy all the sheep on the map
        foreach (Sheep sheep in FindObjectsOfType<Sheep>())
        {
            Destroy(sheep.gameObject);
        }

        // Stop the sheep spawner from spawning more sheep
        sheepSpawner.canSpawn = false;
        
        // Load the game over scene
        SceneManager.LoadScene(2);
    }
}
