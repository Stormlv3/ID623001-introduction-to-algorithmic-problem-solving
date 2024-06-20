using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private MazeMeshGenerator mazeMeshGenerator;
    [SerializeField] private MazeConstructor mazeConstructor;

    [SerializeField] private GameObject playerControllerPrefab;
    [SerializeField] private GameObject monsterPrefab;
    [SerializeField] private GameObject treasurePrefab;

    private int[,] mazeData;
    private int rows;
    private int cols;


    public void Start()
    {
        // Initialize our game state
        mazeData = mazeConstructor.GenerateMazeDataFromDimensions(rows, cols);

        mazeMeshGenerator.GenerateMaze(mazeData);

        // generate the player at the start of the game
        GeneratePlayer();

        // generate the monster at the end of the maze
        GenerateMonster();

        // generate the treasure in a reasonable distance from the player's spawn point
        GenerateTreasure();
    }

    public void GeneratePlayer()
    {
        // coordinates for the player's starting position
        int xCoord = 1;
        int zCoord = 1;
        // calculate the starting position for the player based on maze cell width
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 1, zCoord * mazeMeshGenerator.width);

        // instantiate the player controller at the calculated starting position
        GameObject player = Instantiate(playerControllerPrefab, startPos, Quaternion.identity);
    }

    public void GenerateMonster()
    {
        // Ensure rows and cols are initialized correctly
        int rows = mazeConstructor.Data.GetLength(0);
        int cols = mazeConstructor.Data.GetLength(1);
        // -1 would place the monster inside the final cell, which is closed. 
        // -2 places it in the first guaranteed open cell (assuming your maze has odd-numbered rows + cols)
        int xCoord = rows - 2;
        int zCoord = cols - 2;
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
        GameObject newMonster = Instantiate(monsterPrefab, startPos, Quaternion.identity);
    }

    private void GenerateTreasure()
    {
        // Ensure rows and cols are initialized correctly
        int rows = mazeConstructor.Data.GetLength(0);
        int cols = mazeConstructor.Data.GetLength(1);

        // Assuming your maze is at least 6x6.
        int xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
        int zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);

        // Keep trying random cells until we find an open one or we've tried 1000 times.
        int repeats = 0;
        while (mazeConstructor.Data[zCoord, xCoord] != 0 && repeats < 1000)
        {
            xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
            zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);
            repeats++;
        }

        // Spawn the treasure.
        Vector3 treasurePos = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
        GameObject treasure = Instantiate(treasurePrefab, treasurePos, Quaternion.identity);
    }

}