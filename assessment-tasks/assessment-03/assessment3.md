# 03 Procedural Generation Assessment Tasks

For these assessment tasks, we will make the monster utilize the pathfinding algorithm we have written in order to hunt down the player in a convincing way. We will also add win and loss states to the game. These tasks will require independent planning, research and debugging to complete.

## Monster Pathfinding

To complete this task:
- The monster should pathfind towards the player, chasing them through the maze at an appropriate speed.
- The monster should rotate to face the direction it is moving at all times.

Tips:
- Your `Pathfinder` script should store public references to the `Player` and `Monster` game objects. These should be populated by `GameSetup` when it initializes the level. You can stop these fields from displaying in the inspector with the `[HideInInspector]` tag.
- In the pathfinder's `Update` loop, you should calculate the path from the monster to the player and then make the monster start moving along that path.
- You will need to translate back and forth between cell coordinates and world coordinates. You can multiply a coordinate's **x value** by the **width** of the maze mesh generator to get the **world z** position. You can do the same with the coordinate's **y position** to get the **world x position**.
- If you need to debug your pathfinding, you can use the `LineRenderer` code that was provided in the `Start` method. This is useful for seeing a visualization in-game of how the monster is calculating its path.

If you have implemented this feature successfully, your monster should navigate through the generated maze and move towards the player's position along a sensible path at a balanced speed. The monster should also rotate to face the direction it is moving.

## Win & Loss States

Setup:
- Replace all the code in `TriggerEventRouter.cs` with the following:

```csharp
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventRouter : MonoBehaviour
{
    public UnityEvent OnPlayerEntered = new UnityEvent();
    public UnityEvent OnPlayerExited = new UnityEvent();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEntered?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExited?.Invoke();
        }
    }
}

```

This script can be attached to a game object that has a `collider` component with `IsTrigger` enabled. It will fire different events when the player enters and exits the attached trigger collider.

For this task, you will need to use `TriggerEventRouter` to set up win and loss states for your game.

All of the following functionality should be implemented for this task to be completed:
- If the player reaches the treasure, the game is won.
    - Disable the player's controls
    - Disable the monster's pathfinding
    - Disable the treasure game object
    - Display some text on the UI saying "You Escaped!" for a few seconds
    - Restart the game after a delay
- If the player is caught by the monster, the game is lost.
    - Disable the player's controls
    - Display some text on the UI saying "You Were Caught!" for a few seconds
    - Restart the game


Tips:
- Ensure your player is assigned the "Player" tag, either on the prefab or when it is instantiated.
- Since the treasure game object doesn't exist in the scene until runtime, we will need to add an event callback from `GameSetup`. It should use the `AddListener` method on the treasure's `TriggerEventRouter` when it instantiates the treasure.
- It is the same case for the monster.
- You can use Coroutines to perform logic over time.

> Resource: <https://docs.unity3d.com/ScriptReference/Events.UnityEvent.html>

> Resource: <https://docs.unity3d.com/ScriptReference/Coroutine.html>


## Pathfinding Polish

In our current pathfinding iteration, it is possible for the player to stand at the far side of a node and have the monster stop just short of catching them. To test this out, stand pressed up against the back corner when you begin the game, and watch as the monster comes up short in front of you. This is pretty immersion-breaking, so we should fix it.

To complete this task, your monster should not stop when it reaches the cell the player is on, but continue directly to the player's position to ensure that the player is properly caught. If the player moves out of the cell that the monster is on, it should resume pathfinding as usual. **The monster should only move directly to the player's location when the monster and the player occupy the same cell**.

## Maze Variability

The size of the maze should be random each time the scene runs. There will need to be limitations on this size, however. Generally, anything larger than 25x25 starts to feel too big (and can lead to performance issues), and anything smaller than 9x9 leaves no room to avoid the monster.

To complete this task, your game should generate a maze with random dimensions each time the scene is loaded, with some considerations:
- Each dimension should be constrained between 9-25 cells
- The maze should still only be allowed to have an odd number of cells in both dimensions
- Ensure that the monster and treasure spawns take these new dimensions into account when determining their spawn locations

## Audio Cues

The game is a lot more scary with some sound effects. Having the monster growl also gives the player information about where it might be. You must implement a monster growl that plays periodically and utilizes Unity's 3D sound settings to drive fear into your player's hearts.

Setup:
- Find the attached growl sound effect in the Resources folder.
- Attach an `AudioSource` component to the monster prefab. This audio source should play the growl sound effect and have **Spatial Blend** set to **3D**. You should also adjust the Min and Max Distance for the Volume Rolloff as you see fit.

You will need to write a script that sits on the monster and plays the sound effect from the `AudioSource` on a timer. You can use a Coroutine with a `while(true)` loop to accomplish this.

- There are also audio files for the player's death and the player's escape. Make these sound effects play when the player is caught and escapes respectively.
