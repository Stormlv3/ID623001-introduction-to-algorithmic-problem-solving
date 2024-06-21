/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script places objects in the maze, namely the player, treasure, and monster.
/// It also manages the win and loss states of the game.

using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    public static GameSetup Instance;

    [SerializeField] private MazeMeshGenerator mazeMeshGenerator = null;
    [SerializeField] private MazeConstructor mazeConstructor = null;

    [SerializeField] private GameObject playerControllerPrefab = null;
    [SerializeField] private GameObject monsterPrefab = null;
    [SerializeField] private GameObject treasurePrefab = null;

    [SerializeField] private Text UIText;

    private int[,] mazeData;
    private int rows;
    private int cols;

    private GameObject playerInstance;
    private GameObject monsterInstance;
    private GameObject treasureInstance;

    public GameObject pathFinderScript;

    private bool gameEnded = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        mazeData = mazeConstructor.GenerateMazeDataFromDimensions(rows, cols);

        // Generate the maze mesh from the data
        mazeMeshGenerator.GenerateMaze(mazeData);

        // Generate the player at the start of the game
        GeneratePlayer();

        // Generate the monster at the end of the maze
        GenerateMonster();

        // Generate the treasure
        GenerateTreasure();
    }

    public void GeneratePlayer()
    {
        // Coordinates for the player's starting position
        int xCoord = 1;
        int zCoord = 1;
        // Calculate the starting position for the player
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 1, zCoord * mazeMeshGenerator.width);

        // Instantiate the player
        playerInstance = Instantiate(playerControllerPrefab, startPos, Quaternion.identity);
    }

    public void GenerateMonster()
    {
        int rows = mazeConstructor.Data.GetLength(0);
        int cols = mazeConstructor.Data.GetLength(1);
        // Set coordinates for the monster's starting position near the end of the maze
        int xCoord = rows - 2;
        int zCoord = cols - 2;
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
        monsterInstance = Instantiate(monsterPrefab, startPos, Quaternion.identity);
    }

    private void GenerateTreasure()
    {
        int rows = mazeConstructor.Data.GetLength(0);
        int cols = mazeConstructor.Data.GetLength(1);

        // Set initial coordinates for the treasure's position
        int xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
        int zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);

        // Keep trying random cells until we find an open one or we've tried 500 times
        int repeats = 0;
        while (mazeConstructor.Data[zCoord, xCoord] != 0 && repeats < 500)
        {
            xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
            zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);
            repeats++;
        }

        // Spawn the treasure
        Vector3 treasurePos = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
        treasureInstance = Instantiate(treasurePrefab, treasurePos, Quaternion.identity);
    }

    public void GameWon()
    {
        // Set gameEnded flag to true
        gameEnded = true;

        // Display win text on UI
        UIText.text = "You Escaped!";
        UIText.gameObject.SetActive(true);

        // Play audio
        PlayTreasureAudio();

        // Disable controls
        DisablePlayerControls();

        // Disable monster pathfinding
        DisableMonsterPathfinding();

        // Restart the game
        Invoke("RestartGame", 3f);
    }

    public void GameLose()
    {
        gameEnded = true;

        // Display lose text
        UIText.text = "You Were Caught!";
        UIText.gameObject.SetActive(true);

        // Play audio
        PlayPlayerAudio();

        // Disable controls
        DisablePlayerControls();

        // Restart the game
        Invoke("RestartGame", 3f);
    }


    private void PlayPlayerAudio()
    {
        if (playerInstance != null)
        {
            AudioSource audioSource = playerInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // Play the audio source on the player
                audioSource.Play();
            }
        }
    }

    private void PlayTreasureAudio()
    {
        if (treasureInstance != null)
        {
            AudioSource audioSource = treasureInstance.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                // Play the audio source on the treasure
                audioSource.Play();
            }
        }
    }

    // Disable the player controls when the player wins/looses
    private void DisablePlayerControls()
    {
        if (playerInstance != null)
        {
            FpsMovement playerController = playerInstance.GetComponent<FpsMovement>();
            if (playerController != null)
            {
                // Disable the player controller script
                playerController.enabled = false;
            }
        }
    }

    // Disable the monster pathfinding when the player wins
    private void DisableMonsterPathfinding()
    {
        if (pathFinderScript != null)
        {
            PathFinder monsterPathfinding = pathFinderScript.GetComponent<PathFinder>();
            if (monsterInstance != null)
            {
                monsterPathfinding.enabled = false;
            }
        }
    }


    private void RestartGame()
    {
        // Reload the scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
