# 02: Game Mechanics - Game Management

## Game Managers

We have briefly touched on Game Managers in the previous assessment, and today we are going to implement one for our Tower Defense game. A Game Manager is, in essence, just a high level class that we put in charge of controlling the game's flow. 

A Game Manager can do anything from loading levels to keeping score to initializing scenes. As with anything, it's up to you as the developer to decide what your Game Manager should be allowed to do.

## Task 1 - Part 1: Managing Finances

Currently, our game has no player resources. This means that the only limit to how many towers we can place is how many slots are available on the map. This makes the strategy of the game extremely 1-dimensional: place towers on all the available slots and upgrade them all immediately.

Adding a resource (gold) to our game will limit the player's choices, introducing a much-needed element of strategy.

Set up a new game object in your GameScene and call it `GameManager`. Create a `GameManager.cs` script and attach it to your object.

This GameManager should do a couple of things:
- Act as a singleton.
- Store how much gold we currently have available.
- Fire an event when the gold value changes.

Try to implement this game manager yourself using the concepts we have learned in class. Implement the **Singleton** pattern to make the Game Manager a singleton, and use an **accessor** to fire an event whenever the gold value is set.

**You know the drill:**

<details>
<summary>Solution Example</summary>

```csharp
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UnityEvent OnGoldSet = new UnityEvent();
    public int StartingGold = 1000;// Arbitrary starting value. 
    private int gold;
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            // Anything in our game can hook into this event and run a function whenever the gold
            // changes.
            OnGoldSet?.Invoke();
        }
    }

    private void Awake()
    {
        // Just ensures there can only ever be one instance of Game Manager.
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Gold = StartingGold; 
    }
}
```

</details>

## Task 1 - Part 2: Show Me The Money

Storing a value for gold in the Game Manager is great, but pretty useless if we don't use it for anything. There's also no way for the player to see what this value is while they are playing the game. This is where our user interface comes in.

There is already a UI setup for our game under the `Canvas` game object. This is a good time - if you haven't already - to set the game's resolution to 4:3. You can do this from the Game window. This will ensure that the UI is laid out correctly.

Add a new script to the `Canvas` game object and call it something like `UserInterface.cs`. This script will be in charge of updating the UI whenever appropriate.

Your next task is to make `UserInterface` display the correct gold amount whenever the player's gold changes. There is already a `Text` object set up for you to write the gold value to.

> Resource: <https://docs.unity3d.com/2017.3/Documentation/ScriptReference/UI.Text-text.html>

<details>
<summary>Solution Example</summary>

Make sure you assign `goldLabel` in the editor.

```csharp
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private Text goldLabel; 

    private void Start()
    {
        GameManager.Instance.OnGoldSet.AddListener(HandleGoldSet);
        HandleGoldSet(); // Ensure the gold is set correctly when this object starts.
    }

    private void HandleGoldSet()
    {
        goldLabel.text = "GOLD: " + GameManager.Instance.Gold.ToString();
    }
}
```

</details>

## Task 2: Making It Matter

We can see our gold score on the user interface, but it doesn't update when we place a monster. It's still useless, but not for long!

You must now update your `MonsterSlot` script to:
- Check how much gold is currently available when the player attempts to place a monster.
- Only let the player place a monster if they have the gold for it.
- When a monster is successfully placed, the cost of that monster should be deducted from the player's total gold.

<details>
<summary>Solution Example</summary>

I've divided out the decision-making / execution sections of the code to make it more readable. This is generally good practice, especially because as our game grows in complexity, we may want to be able to spawn or upgrade monsters from other scripts!

```csharp
using UnityEngine;

public class MonsterSlot : MonoBehaviour
{
    public MonsterData MonsterPrefab;
    private MonsterData placedMonster = null;

    void OnMouseUp()
    {
        if (placedMonster == null)
        {
            if (CanPlaceMonster())
            {
                PlaceMonster();
            }

        }
        else if (CanUpgradeMonster())
        {
            UpgradeMonster();
        }
    }

    private bool CanUpgradeMonster()
    {
      // This is a conditional that checks multiple things:
      // - Is there a monster currently placed?
      // - Does that monster have another level available? (it's not maxxed out)
      // - Do we have enough gold to perform an upgrade?
      // All of these conditions have to be true for this function to return true,
      // otherwise it will return false.
        return placedMonster != null &&
            placedMonster.GetNextLevel() != null &&
            GameManager.Instance.Gold >= placedMonster.GetNextLevel().cost;
    }

    public void UpgradeMonster()
    {
        placedMonster.IncreaseLevel();
        GameManager.Instance.Gold -= placedMonster.CurrentLevel.cost;
    }

    private bool CanPlaceMonster()
    {
        return placedMonster == null && GameManager.Instance.Gold >= MonsterPrefab.levels[0].cost;
    }

    public void PlaceMonster()
    {
        placedMonster = Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
        GameManager.Instance.Gold -= MonsterPrefab.levels[0].cost;
    }
}
```

</details>

## Task 3: Wayfinding

**Pathfinding** is a commmon algorithm used in many different game genres, and is something that we will explore in more detail in later classes. For now, we are going to implement a more rudimentary method for navigating our enemies through the game world; moving directly between predetermined waypoints (hence the term "wayfinding").

A waypoint can be defined as simply a point in world space. An easy way to define a useful point in your game is with an empty game object.

Set up 5 waypoints in your scene at each corner of the road (and one at the cookie).

Create a new script and attach it to your enemy prefab. 
- This script should make the enemy move between each waypoint in sequence, before stopping at the final waypoint.

> Hint: A useful method to know about is Unity's `Vector2.MoveTowards()` function. It might help you solve this problem: <https://docs.unity3d.com/ScriptReference/Vector2.MoveTowards.html>
> You can also use `Vector2.Distance()` to tell how far apart two vectors are. Use this when determining if the enemy has reached a waypoint or not. <https://docs.unity3d.com/2017.3/Documentation/ScriptReference/Vector2.Distance.html>

<details>
<summary>Solution Example</summary>

This solution assumes you have provided a list of Transforms in the `waypoints` field. The enemy will move through these in order.

```csharp
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MoveSpeed = 5;
    public Transform[] waypoints;
    private int currentWaypointIndex;
    
    private void Update()
    {
        if (currentWaypointIndex == waypoints.Length)
        {
            // We've reached the end, so do nothing.
            return;
        }

        Transform toWaypoint = waypoints[currentWaypointIndex];
        // MoveTowards is a really useful inbuilt Unity function for doing this kind of thing.
        Vector2 moveVector = Vector2.MoveTowards(transform.position, toWaypoint.position, MoveSpeed * Time.deltaTime);
        transform.position = (Vector3)moveVector;

        if (Vector2.Distance(transform.position, toWaypoint.position) <= float.Epsilon)
        {
            // We reached our target waystation.
            currentWaypointIndex++;
        }
    }
}
```

</details>

Place your enemy prefab in the scene and ensure that it moves through the waystations as intended when you run your game.

## Task 4: Spawning

Lastly, set up an enemy spawner like we did with the sheep spawner in the previous project. Bear in mind that the enemy spawner will need to provide the enemies with the list of waypoints for them to navigate when they are instantiated.

<details>

<summary>Solution Example</summary>

This is just a slick and easy to write solution. You could achieve the same result with an `Update()` loop and a `float` that keeps track of the last time an enemy was spawned, then spawn an enemy if the time is greater than our previous spawn time plus a spawn delay. This is an example of how there are generally multiple ways to tackle any problem.

If you are unfamiliar with coroutines, here is an explanation video that covers them: <https://www.youtube.com/watch?v=kUP6OK36nrM>

```csharp
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float EnemySpawnDelay = 2f;
    public Enemy EnemyPrefab;
    public Transform[] waypoints;

    // This is a cool Unity trick where you can define Start as a coroutine.
    // Unity will automatically run it as a coroutine when the game object starts.
    IEnumerator Start ()
    {
        // while(true) is powerful but dangerous - it'll crash your code 
        // if there's no way to break out of the loop! Thankfully our yield statement
        // breaks us out and waits an amount of time before this code runs again,
        // but if we didn't have that, it would execute infinite times in one frame
        // until we had a stack overflow and our whole program crashed. You can try it
        // if you want to crash Unity and have a bad time.
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(EnemySpawnDelay);
        }
    }

    public void SpawnEnemy()
    {
        Enemy newEnemy = Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
        newEnemy.waypoints = waypoints;
    }
}

```

</details>
