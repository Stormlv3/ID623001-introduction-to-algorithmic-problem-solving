using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    // a variable to display a debug of the maze
    public bool showDebug;
    // maze data
    public int[,] Data { get; private set; }

    public int rows, cols;

    public Node[,] Graph { get; private set; }

    void Awake()
    {
        GenerateNewMaze(rows, cols);
    }

    // gui = graphical user interface
    void OnGUI()
    {
        // if showDebug is not ticked, nothing will happen
        if (!showDebug)
            return;

        // get the maze data to display
        int[,] maze = Data;
        // get maximum index for the rows.
        int rMax = maze.GetUpperBound(0);
        // get maximum index for the columns.
        int cMax = maze.GetUpperBound(1);

        // an empty string to build the visual representation of the maze.
        string msg = "";

        // loop through the rows from the first index to the last (top to bottom)
        for (int i = 0; i <= rMax; i++)
        {
            // loop through the columns from the first index to the last (left to right)
            for (int j = 0; j <= cMax; j++)
            {
                // if the cell is 0 (is an open space), add "....", if the cell is 1 (is a wall), add "=="
                msg += maze[i, j] == 0 ? "...." : "==";
            }
            // adds a new line at the end of each row to separate rows visually
            msg += "\n";
        }

        // displays the maze string on the screen as a label
        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

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
                else if (x % 2 == 0 && y % 2 == 0)
                {
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

    public void GenerateNodeGraph(int sizeRows, int sizeCols)
    {
        Graph = new Node[sizeRows, sizeCols];

        for (int i = 0; i < sizeRows; i++)
        {
            for (int j = 0; j < sizeCols; j++)
            {
                bool isWalkable = (Data[i, j] == 0);
                Graph[i, j] = new Node(i, j, isWalkable);
            }
        }

        Debug.Log(GenerateGraphDebugString());
    }

    public void GenerateNewMaze(int numRows, int numCols)
    {
        rows = numRows;
        cols = numCols;
        Data = GenerateMazeDataFromDimensions(rows, cols);
        GenerateNodeGraph(rows, cols);
    }

    private string GenerateGraphDebugString()
    {
        string graphString = "Node Graph:\n";

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                graphString += Graph[i, j].isWalkable ? "O " : "X ";
            }
            graphString += "\n";
        }

        return graphString;
    }
}