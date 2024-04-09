# 01: Procedural Generation

**Project 3** is going to be more technical than the previous 2 projects, utilizing more complicated algorithms. To compensate for this, we will be spending extra time in class to fully understand the necessary concepts. It is recommended you attend the classes, as we will be exploring **proc-gen** concepts collaboratively.

For this project, we are going to develop a **horror game** that utilizes **procedural generation** to **create a maze** that the player must navigate while being pursued by a relentless monster.

## What is Procedural Generation?

**Procedural Generation** (or proc-gen) is the method of using **algorithms** to **generate content** for our game. It allows us to offload the responsibility of creating content for our game to the game engine.

- Incorporates **RNG** (Random Number Generation)
- Can be used to generate **anything** (level layouts, enemy types, item spawns etc.)
- Provides potentially **infinite permutations**

Proc-gen isn't a magical cure-all for game design. When implemented well, incredibly interesting and memorable gameplay scenarios can arise, but it has its drawbacks. When implemented poorly, proc-gen can severely impact players' enjoyment of your game by leading to unfair or frustrating scenarios.

Anything we can do in the editor, we can also do **in code**.
A script could do any of the following at runtime:

- **Generate** game objects
- **Add components** to game objects
- **Change properties** of existing game objects

Introducing randomness to these procedures is the core of proc-gen.

## Maze Generation

Initially, it may seem a little overwhelming to consider how we would generate a maze in Unity. Afterall, how would we even go about creating the necessary game objects?

It is helpful to take a step back and think about our maze in a more abstract way, before we worry about the details of how we would create it in Unity. This will be our **main focus** for today.

A maze may seem complicated, but the way we frame our thinking about it can actually make it pretty simple. Rather than a contiguous 3D space, we can think about a maze as a **set of cells** in space. A cell is just a **point** that has some **size** (think like a square on a chessboard).

In our simple maze generation algorithm, we want to randomly generate some **data** that represents each of these **cells**. Each cell can either be **"open"** i.e. there are no walls there, or the point can be **"closed"**, i.e. the cell is blocked off by walls.

**We will do a collaborative problem solving session to come up with a structure for our maze data.**

<details>
<summary>Solution</summary>

Our data should be structured as a matrix of **binary values**, where the value `0` represents an open cell, and the value `1` represents a closed cell.

e.g.

```csharp

1 1 1 1
1 0 0 1
1 1 1 1

1 1 1 1 1 1 1
1 0 1 1 0 0 1
1 0 0 0 0 0 1
1 1 1 1 1 1 1

```

You can likely already see how these matrices resemble a maze: The closed cells marked `1` represent the walls of the maze, and the open cells marked `0` represent a path that we could navigate.

</details>


## Task 1: Visually Representing Data

It will help for the next few tasks if we can visualize the data that we are working with.

In the project, create a new script called `MazeConstructor.cs` and add it to an empty game object. You may use the following code as a starting point:

```csharp
using UnityEngine;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private int[,] data;

    void Awake()
    {
        // TODO: Define a default value for data.
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
}

```

The `OnGUI` function is just there to help us visualize our data when we run the game. Its exact workings aren't overly important, but you can ask me about it if you'd like to understand it in more detail.

What's important here is the `data` field. This is of type `int[,]`, which means it is a **2D array of integers**. This is equivalent to a **matrix** in mathematics. If you haven't seen 2D arrays, they are essentially just an **array of arrays** i.e. each element in the array **is itself** an array. An array could be any number of dimensions, but to keep our brains from hurting too much, we will only need to work with 2.

> ONGUI should only really be used for debugging. It's not a replacement for Unity's in-depth UI system. You don't need to know about it, but for those curious:
> Resource: <https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnGUI.html>

Task: In your `Awake` method, where there is a comment marked "todo", specify some data for your maze. At this stage, you are just expected to hardcode it. Run the game and you should see a representation of your data appear on the screen, where a "...." indicates an open cell and a "==" indicates a closed cell.


## Task 2: Randomly Generating Data

We have determined a format for our generation data, but currently the data is the same every time we run the game. The next step is to **introduce RNG** to our data generation.

**We will do a collaborative problem solving session to come up with some rules for our data generation**


<details> 
<summary>Solution Example</summary>

We will use the following rules when generating our data:

- Our data matrix can be of **variable width and height**
- Every cell on the edge of the matrix should be **"closed"**
- Every cell with an **even** x and y co-ordinate will have a **chance** of being **closed**.
- If an even cell is **closed**, another **adjacent** cell (not diagonally) will also be **closed**.

There are more sophisticated methods of generating mazes (if you want to do some reading outside of class, check out the resource below. It references a lot of computer science concepts), but this algorithm will suffice for our simple game.

> Resource: <https://en.wikipedia.org/wiki/Maze_generation_algorithm>

</details>

## Implementation

Your task now is to implement the following method in `MazeConstructor`, utilizing the generation rules we have established:

```csharp

public int[,] GenerateMazeDataFromDimensions(int numRows, int numCols)
{
    int[,] maze = new int[numRows, numCols];
    float placementThreshold = 0.1f; // How likely we are to place a cell (0.1 == a 90% chance)
    
    // TODO:
    // Iterate over every cell in the maze (this will require a nested for loop)
    //     - If the cell we are considering is on the edge of the maze, make it closed (i.e. set the value in the matrix to 1)
    //     - Otherwise, if the column and row indexes are even:
    //          - Generate a random value and compare it to the placement threshold. If it exceeds the threshold:
    //              - Make the current cell closed
    //              - Choose a random adjacent cell (vertically or horizontally, not diagonally) and also set it to closed.

    return maze;
}
```

You may then call this method from the `MazeConstructor`'s `Awake` method, and set `data` to the result. If implemented correctly, you should get a different maze every time you start the game, which follows the data generation rules we have established.

<details>
<summary>Solution Example</summary>

```csharp

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

```

This code can be made more concise, but it is written out a little verbosely here to better explain what's happening.

</details>

## Conclusion

We have now procedurally generated some data! In the next class, we will use this procedurally generated data to construct a 3D maze, which we will ultimately be able to navigate through (and be chased) in first-person!