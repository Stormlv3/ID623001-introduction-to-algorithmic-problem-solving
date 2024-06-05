using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private int[,] data;

    void Awake()
    {
        data = GenerateMazeDataFromDimensions(30, 30); // Example size, you can change it
    }

    void OnGUI()
    {
        if (!showDebug)
            return;

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
                msg += maze[i, j] == 0 ? "...." : "==";
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    public int[,] GenerateMazeDataFromDimensions(int numRows, int numCols)
    {
        int[,] maze = new int[numRows, numCols];
        float placementThreshold = 0.1f; // How likely we are to place a cell (0.1 == a 90% chance).

        for (var x = 0; x < numRows; x++) // Iterate over rows.
        {
            for (var y = 0; y < numCols; y++) // Iterate over columns.
            {
                if (x == 0 || y == 0 || x == numRows - 1 || y == numCols - 1) // If we're on an edge...
                {
                    maze[x, y] = 1; // Close this cell.
                }
                else if (x % 2 == 0 && y % 2 == 0) // Otherwise, if row and col are even...
                {
                    if (Random.value > placementThreshold) // ...and we pass the placement threshold...
                    {
                        maze[x, y] = 1; // Close this cell.

                        // Pick an offset value (either 1 or -1).
                        int delta = Random.value > 0.5f ? -1 : 1;
                        // Define an offset vector [x,y].
                        int[] offset = new int[2];
                        // Decide whether to apply the offset value to x or y. (0 == x, 1 == y)
                        int offsetIndex = Random.value > 0.5f ? 0 : 1;
                        offset[offsetIndex] = delta;

                        // Close the adjacent cell.
                        maze[x + offset[0], y + offset[1]] = 1;
                    }
                }
            }
        }

        return maze;
    }
}
