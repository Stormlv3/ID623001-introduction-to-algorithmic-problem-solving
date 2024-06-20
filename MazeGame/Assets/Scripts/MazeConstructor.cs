/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script creates the procedural maze which will later be turned into a physical maze.

using UnityEngine;

public class MazeConstructor : MonoBehaviour
{

    public bool showDebug;
    public int[,] Data { get; private set; }
    public int rows, cols;

    // The node graph representation of the maze
    public Node[,] Graph { get; private set; }

    void Awake()
    {
        // Generate a new maze when the game starts
        GenerateNewMaze(rows, cols);
    }

    // Method to draw the maze in the GUI for debugging
    void OnGUI()
    {
        if (!showDebug)
            return;

        // Get the maze data to display
        int[,] maze = Data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        string msg = "";

        // Loop through the rows of the maze
        for (int i = 0; i <= rMax; i++)
        {
            // Loop through the columns of the maze
            for (int j = 0; j <= cMax; j++)
            {
                // Add symbols to represent open spaces and walls
                msg += maze[i, j] == 0 ? "...." : "==";
            }
            // Add a new line at the end of each row
            msg += "\n";
        }

        // Display the maze string on the screen as a label
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    // Method to generate maze data based on dimensions
    public int[,] GenerateMazeDataFromDimensions(int numRows, int numCols)
    {
        int[,] maze = new int[numRows, numCols];
        float placementThreshold = 0.1f;


        for (var x = 0; x < numRows; x++)
        {

            for (var y = 0; y < numCols; y++)
            {

                if (x == 0 || y == 0 || x == numRows - 1 || y == numCols - 1)
                {
                    maze[x, y] = 1;
                }
                // Create walls at even intervals
                else if (x % 2 == 0 && y % 2 == 0)
                {
                    // Randomly place walls based on the threshold
                    if (Random.value > placementThreshold)
                    {
                        maze[x, y] = 1;

                        int delta = Random.value > 0.5f ? -1 : 1;
                        int[] offset = new int[2];
                        int offsetIndex = Random.value > 0.5f ? 0 : 1;
                        offset[offsetIndex] = delta;

                        maze[x + offset[0], y + offset[1]] = 1;
                    }
                }
            }
        }

        return maze;
    }

    // Method to generate the node graph representation of the maze
    public void GenerateNodeGraph(int sizeRows, int sizeCols)
    {
        Graph = new Node[sizeRows, sizeCols];


        for (int i = 0; i < sizeRows; i++)
        {

            for (int j = 0; j < sizeCols; j++)
            {
                // Determine if the node is walkable based on the maze data
                bool isWalkable = (Data[i, j] == 0);
                Graph[i, j] = new Node(i, j, isWalkable);
            }
        }

        // Debug log to show the graph string
        Debug.Log(GenerateGraphDebugString());
    }

    // Method to generate a new maze
    public void GenerateNewMaze(int numRows, int numCols)
    {
        rows = numRows;
        cols = numCols;
        Data = GenerateMazeDataFromDimensions(rows, cols);
        GenerateNodeGraph(rows, cols);
    }

    // Method to generate a debug string representation of the node graph
    private string GenerateGraphDebugString()
    {
        string graphString = "Node Graph:\n";


        for (int i = 0; i < rows; i++)
        {

            for (int j = 0; j < cols; j++)
            {
                // Add symbols to represent walkable and non-walkable nodes
                graphString += Graph[i, j].isWalkable ? "O " : "X ";
            }
            // Add a new line at the end of each row
            graphString += "\n";
        }

        return graphString;
    }
}
