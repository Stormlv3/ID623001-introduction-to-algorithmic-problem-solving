# 03: Game Setup

We have successfully used procedural generation to create a random 3D maze for our game!

Today's class will be focused on adding some fundamental gameplay mechanics to our game, including player controls and object spawning. Afterall, what use is a maze that we can't explore?

## Mechanics

Today we will implement the following mechanics to the maze game:
- Player controls + movement
- Player, monster and treasure spawning

## Player Controls

There is already a player prefab set up in the project to help you with this task. The prefab can be found in **Assets -> Prefabs -> Player**. You may find that it has a broken script attached to it. If this is the case, remove the broken script reference, and add the `FPSController` component to the root player object. You will need to assign the camera that is childed to the player to the `HeadCam` field in the `FPSController`. 

This component utilizes Unity's inbuilt `CharacterController` component to handle player movement. `CharacterController` can often be an easier and less bug-prone way to manage player movement as opposed to dealing with a rigidbody and the physics system.

Resouce: <https://docs.unity3d.com/ScriptReference/CharacterController.html>

Once your player prefab is set up correctly, try placing it in the scene and running the game. If you can position it so that the player falls into the generated maze, you may even be able to run around inside it.

Obviously, this isn't an ideal setup, so we are going to position the player in code when the game starts.

## Task 1: Positioning the Player

To complete this task, your player prefab should be dynamically instantiated in the scene and positioned correctly in the center of the first open slot of the maze, so that when the game is played, the player is immediately standing on the floor at the start of the maze.

Now that we are starting to deal with multiple different systems (maze generator + player positioning), consider how your code should be structured. Should it be the duty of the maze generator to position the player? Or should there be some greater system that co-ordinates the level setup?

> Tip: In order to position the player in the center of a maze cell, we need to take into account the in-game size of the maze cells, as well as the co-ordinate we wish to elect as the starting co-ordinate.

If your player prefab is setup correctly, and instantiated correctly in code, you should be able to navigate the maze in first-person when playing the game.

<details>
<summary>Solution Example</summary>

Now that our code is growing more complex, we should consider how it is structured. The **Single Responsibility Principle** states that each class in our program should only be have one job. It doesn't really make sense that our maze generator should be performing other initialization tasks such as setting up the player, when all it should care about is creating the maze.

Create a class called `GameSetup` (or something along those lines, if you haven't already created a class to handle initialization). The class should look something like this: 


```csharp
using UnityEngine;

public class GameSetup: MonoBehaviour 
{
    [SerializeField] private int rows;
    [SerializeField] private int cols;

    [SerializeField] private MazeMeshGenerator mazeMeshGenerator;
    [SerializeField] private MazeConstructor mazeConstructor;

    public void Start() 
    {
        // Initialize our game state
        mazeConstructor.GenerateNewMaze(rows, cols);

        GeneratePlayer();
    }

    public void GeneratePlayer() 
    {
        int xCoord = 1;
        int zCoord = 1;
        Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 1, zCoord * mazeMeshGenerator.width);
    
        // Obviously, this is overly verbose. We could simplify to:
        // Vector3 startPos = new Vector3(mazeMeshGenerator.width, 1, mazeMeshGenerator.width);
        // And get the same result. The point here is that we can multiply the maze cell co-ordinate with the mesh cell size to
        // get the world-space coordinate of any cell in our maze. This concept will come in handy for the next task.

        GameObject player = Instantiate(playerController, startPos, Quaternion.identity);
    }
}

```


</details>

## Task 2: Adding the Monster

Completing this task will be similar to the first. While the player is placed at the start of the maze, however, the monster should be placed at the end of the maze. This means the maze cell with the maximum co-ordinates should be chosen as the monster's spawn location. Setup the monster prefab - you can replace its model with something else if you want to give your game a unique flair - and instantiate/position it in code as we did with the player.

<details>
<summary>Solution Example</summary>

```csharp

public void Start() 
{
    // Initialize our game state
    mazeConstructor.GenerateNewMaze(rows, cols);

    GeneratePlayer();
    GenerateMonster();
}


public void GenerateMonster()
{
    // -1 would place the monster inside the final cell, which is closed. 
    // -2 places it in the first guaranteed open cell (assuming your maze has odd-numbered rows + cols)
    int xCoord = rows-2; 
    int zCoord = cols-2;
    Vector3 startPos = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
    GameObject newMonster = Instantiate(monster, startPos, Quaternion.identity);
}
```

</details>

## Task 3: Adding the Treasure

The final task is to add a goal for the player to reach. You should create the treasure prefab from scratch, using a default cube mesh textured with the "treasure" material. This prefab should be generated in the same way as the monster, although instead of spawning at the end of the maze, it should choose a random open cell that is a reasonable distance away from the player's spawn point.

**Some considerations:**
- If we are choosing a random cell, it could potentially be closed, which would spawn our treasure out of bounds. You will need a way to ensure that the cell chosen for the treasure to spawn in is open.
- A potential solution to this is to pick a random cell within a suitable range, then check if it is open or not. If not, pick another random cell in that range and keep trying until you find an open one.
- The `GameSetup` script will need to be able to read the data generated by the maze constructor (but it shouldn't be allowed to write to it!).


<details>
<summary>Solution Example</summary>

Assuming that you have setup a treasure prefab:

```csharp

public void Start() 
{
    // Initialize our game state
    mazeConstructor.GenerateNewMaze(rows, cols);

    GeneratePlayer();
    GenerateMonster();
    GenerateTreasure();
}


private void CreateTreasure()
{
    // Assuming that your maze is at least 6x6.
    int xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
    int zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);

    // Keep trying random cells until we find an open one or we've tried 1000 times.
    // IMPORTANT: This is assuming a public accessor 'Data' added to the maze constructor.
    int repeats = 0;
    while (mazeConstructor.Data[zCoord, xCoord] != 0 && repeats < 1000)
    {
        xCoord = UnityEngine.Random.Range(rows - 5, rows - 2);
        zCoord = UnityEngine.Random.Range(cols - 5, cols - 2);
        repeats++;
    }

    // Spawn the treasure.
    Vector3 treasurePosition = new Vector3(xCoord * mazeMeshGenerator.width, 0, zCoord * mazeMeshGenerator.width);
    GameObject treasure = Instantiate(treasurePrefab, treasurePosition, Quaternion.identity);
}
```

</details>

## Summary

If you have successfully implemented object spawning, you should have a player that can run around the maze when the game is played, a monster who currently lurks at the end of the maze but doesn't do anything, and a golden treasure that spawns somewhere inside the maze somewhere near the monster.