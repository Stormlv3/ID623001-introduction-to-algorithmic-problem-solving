# 01: Game Mechanics - Accessors, Getters and Setters

## Tower Defense

For this next assessment project, we are going to explore implementing some new gameplay mechanics into a tower defense game.

The aim is to build a 2D tower defense game, including features such as **gaining and using resources**, **upgrading units**, and rudimentary **enemy wayfinding**.

If you are unfamiliar with the **tower defense** genre, search up "Bloons Tower Defense 6". It's made by a New Zealand studio!

## Accessors, Getters and Setters

Sometimes when we are coding, it would be extremely convenient if we could run some additional code every time a variable is changed. Say we had a variable `money`, and every time it increased we wanted to play a "cha-ching" sound effect. Conventionally, we might think to make money `private`, and make sure we remember to only set it through a public method, something like:

```csharp
private int money = 50;
private AudioSource chaChingSFX;

public void SetMoney(int newMoney) 
{
    money = newMoney;
    chaChingSFX.Play();
}

```

This would work fine, but it's a little inconvenient to call this method every time you want to modify `money`. A more convenient way to do this is to write an `accessor` for money.

An accessor looks like this:

```csharp
private int money = 50;
private AudioSource chaChingSFX;

public int Money {
    get { return money; }
    set 
    {  
        money = value;
        chaChingSFX.Play();
    }
}

```

`Money` is a public accessor for the private variable `money`. It behaves like a sort of interface between `money` and us, the programmers - that is - it performs additional instructions when we read or modify the `money` variable. (We modify `money` through `Money`, which I'll show in a moment).

What happens when we read or write `Money` is defined by the `get {}` and `set {}` code blocks. These code blocks are referred to as `getters` and `setters`. Whatever code you put inside the parentheses will be executed when `Money` is read or modified respectively.

You might've noticed that `set { money = value; ...` has something weird about it. Where the heck does `value` come from?

`value` is a special value passed into all setters, which is always equal to whatever value we were trying to set, so if a part of my code said:

```csharp
Money = 125;
```

, then `value` in my `set { }` block would be `125`.

We now interact with the `money` variable through the `Money` accessor, just like we would interact with a normal variable:

```csharp

public void SomeFunction() 
{
    Money = 50; // money will now be 50, cha-ching SFX plays.
    Money += 50; // money is now 100, cha-ching SFX plays again.
    Money = Money - 75; // money is now 25, cha-ching SFX plays a 3rd time.
}

```

As you can see, `Money` is treated just like a normal integer variable, no need to call a method to set it!

> Tip: Once you've created an accessor, you should never modify or access the variable it's accessing directly. This is bad practice and would defeat the whole point of having an accessor!


## Task 1 : Unit Placement

The goal of our game is to place towers (monsters) along the edges of the path to stop bugs from reaching and consuming the cookie. The challenge in tower defense games comes from deciding which towers to choose, and where to position them for optimal defensive capability.

The first feature we are going to add to our tower defense game is monster placement. Provided in your project is an image called "Openspot". Turn this image into a prefab and drop it into GameScene. Call it something like "MonsterSlot".

Your game will need multiple Monster Slots, as each slot will serve as a position for the player to position their monsters. Create multiple instances of your MonsterSlot prefab in the scene. The more slots you add, the more options the player will have. I'd suggest placing 10-12.

Now that the monster slots are set up, each slot will need some interactivity. Initially what we want is for a monster to be spawned at the position of a slot when we click on it. Luckily, there is already a monster prefab set up for us in the project, which we can utilize.

**We will do a collaborative problem solving session in class to discuss potential solutions to this problem.**

Our solution should:
- Spawn a `Monster` prefab on any `MonsterSlot` when that `MonsterSlot` is clicked.
- Not spawn a `Monster` if there is already a `Monster` on that slot.

> Tip: Unity has an inbuilt function called `OnMouseUp()`, which fires when the player clicks on a game object. It could be useful here! <https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnMouseUp.html>

**Attempt to implement your own solution before looking at the solution example.**

<details>
<summary>Solution Example</summary>

Firstly, we will need to spawn the monster prefab! We should instantiate the monster prefab on the slot when the slot is clicked.

Creating a script for the MonsterSlot, and calling it something like `MonsterSlot.cs`, we could start by writing something like this:

```csharp
public class MonsterSlot : MonoBehaviour 
{
  public GameObject MonsterPrefab;

  void OnMouseUp()
  {
      Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
  }
}

```

This isn't going to work right away; that's because, as it says in the documentation, `OnMouseUp()` will only fire on an object that has a collider. We need to assign a collider 2D of any kind to our `MonterSlot` prefab in order for the function to run when we click the slot. Once we've set up a collider, the code will run.

Now we have satisfied our first goal; spawning a monster when a slot is clicked, however we are still able to spawn multiple monsters with subsequent clicks. Obviously, this is going to cause issues for our game's balance.

The most robust solution here is to store a reference to a `GameObject` in the `MonsterSlot`, called something like `PlacedMonster` or `CurrentMonster`. When we instantiate a new monster in this slot, we can set `PlacedMonster` to the reference of the monster we created, meaning the `MonsterSlot` is now aware of which monster is associated with it. Then, the next time we try to add a monster to this slot, the slot can check to see  if it is already associated with a monster. If it is, then we're not allowed to instantiate another one!

```csharp
public class MonsterSlot : MonoBehaviour 
{
  public GameObject MonsterPrefab;
  private GameObject placedMonster = null; // Even if we didn't specify "= null", this will be null by default.

  void OnMouseUp()
  {
    if (placedMonster == null) // 'null' in C# means that the reference doesn't exist.
    {
      // Since placedMonster doesn't refer to anything, we can go ahead and instiate the new
      // monster, then make placedMonster refer to it.
      placedMonster = Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
      // Now the next time this function runs, placedMonster WON'T be null, so we won't create a new one!
    }
  }
}
```
</details>

## Task 2 : Upgrading Monsters

The next feature we would like to add is upgrading monsters. When a monster slot is clicked, if there's already a monster associated with that slot, then we would like to upgrade it. To accomplish this feature, we will need to add a script to the monsters that keeps track of their levels.

You may use the following script as a starting point for this task. This script should be attached to your Monster prefab:

Where there are comments, it is up to you to implement that code block. The comments can be read like pseudo-code, so they will help you structure your solution, but they will not provide you with the actual lines of code.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterLevel
{
    public int cost; // How much it costs to go to this level (we will implement this later)
    public GameObject visualization; // A GameObject that is a child of the monster. There are 3 on the provided prefab.
}

public class MonsterData : MonoBehaviour
{
    public List<MonsterLevel> levels; // Start by defining 3 of these in the editor, 1 for each monster visualization.

    private MonsterLevel currentLevel; // We won't modify this directly, instead we'll use the CurrentLevel accessor.

    // Sets our level to the first level when this GameObject is enabled.
    public void OnEnable()
    {
        CurrentLevel = levels[0];
    }

    // Accessor for the monster's level:
    public MonsterLevel CurrentLevel
    {
        get { return currentLevel; }
        set
        {
            currentLevel = value;

            // ___EXPLANATION___
            // This setter runs whenever we set the monster's CurrentLevel (which we are going to do from the IncreaseLevel() function below.)
            // - We want it to automatically show the correct visualization for the monster based on its level.
            // - We can find that visualization in our "levels" list.
            // _________________

            // __PSEUDO_CODE:__
            // - Get a reference to the visualization that is associated with this MonsterLevel.
            // - Iterate over all of the visualizations for our different levels
            // {
            // - If this is the visualization associated with this MonsterLevel, its gameObject should be set to active.
            // - If this is NOT the visualization associated with this MonsterLevel, its gameObject should be set to INACTIVE.
            // }
            // ________________
        }

    }

    public MonsterLevel GetNextLevel()
    {
        // This function should return the monster level that is 1 higher than our current level (from the "levels" list).
        // If we are already at the highest level (i.e. currentLevel is set to the last level in the "levels" list),
        // then this function should return null.
    }

    public void IncreaseLevel()
    {
        // This function should set CurrentLevel to the next level in our "levels" list, if we're not already at max level.
    }
}
```

This is a more challenging problem than we've faced thus far, and I expect it to push you. Still - take some time to attempt a solution. The process of problem solving is an important one to practice. This is more about thinking algorithmically than getting the "right" answer. If you are stuck on code syntax but think you have a solution, ask me to take a look.

<details>
<summary>Solution Example</summary>

```csharp
[System.Serializable]
public class MonsterLevel
{
    public int cost;
    public GameObject visualization;
}

public class MonsterData : MonoBehaviour
{
    public List<MonsterLevel> levels;

    private MonsterLevel currentLevel;

    public void OnEnable()
    {
        CurrentLevel = levels[0];
    }

    public MonsterLevel CurrentLevel
    {

        get { return currentLevel; }

        set
        {
            currentLevel = value;
            int currentLevelIndex = levels.IndexOf(currentLevel);

            // Finds the visualization associated with the current level.
            GameObject levelVisualization = levels[currentLevelIndex].visualization;

            // Iterates over each level.
            for (int i = 0; i < levels.Count; i++)
            {
                // Sanity check to make sure this level actually has a visualization.
                if (levelVisualization != null) 
                {
                  // If we're considering the current level, make the visualization active
                  if (i == currentLevelIndex) 
                  {
                    levels[i].visualization.SetActive(true);
                  }
                  else 
                  {
                    // Otherwise, make it inactive (if we didn't do this, the old visualization would stay active when you level up.)
                    levels[i].visualization.SetActive(false);
                  }
                }  
            }
        }

    }

    public MonsterLevel GetNextLevel()
    {
        // Gives us the index of the current level, which we can use to access the next one.
        int currentLevelIndex = levels.IndexOf(currentLevel);
        // Checks to make sure we're not already at the max level.
        int maxLevelIndex = levels.Count - 1;
        if (currentLevelIndex < maxLevelIndex)
        {
            // Gives us the next level in our levels list.
            return levels[currentLevelIndex + 1];
        }
        else
        {
            return null;
        }
    }

    public void IncreaseLevel()
    {
        // Gives us the index of the current level, which we can use to access other levels.
        int currentLevelIndex = levels.IndexOf(currentLevel);
        // Checks to make sure we're not already at the max level.
        int maxLevelIndex = levels.Count - 1;
        if (currentLevelIndex < maxLevelIndex)
        {
            CurrentLevel = levels[currentLevelIndex + 1];
        }
    }
}

```

</details>

Now that our monsters can track their levels, our `MonsterSlot` should tell the monster to increase its level when the `MonsterSlot` is clicked (if there's a monster associated with it, and that monster can be upgraded).

The next part of this task is to add to `MonsterSlot` and implement the commented code blocks:

```csharp

public class MonsterSlot : MonoBehaviour 
{
  public GameObject MonsterPrefab;
  private GameObject placedMonster = null;

  void OnMouseUp()
  {
    if (placedMonster == null)
    {
      placedMonster = Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
    }
    else if (CanUpgradeMonster())
    {
      // Level up the placedMonster. Remember that you can use GetComponent<>() to access different components on the same gameObject.
    }
  }

  private bool CanUpgradeMonster()
  {
    // If there is a monster associated with this slot, this function should:
    // - Get the MonsterData component from that monster
    // - Return true if that monster can go to the next level, otherwise return false.
  }
}
```

We have already implemented the required methods in our MonsterData class, now it's just a matter of using them correctly.

**Attempt a solution before you look at the solution example.**

<details>
<summary>Solution Example</summary>

```csharp
public class MonsterSlot : MonoBehaviour
{
    public GameObject MonsterPrefab;
    private GameObject placedMonster = null;

    void OnMouseUp()
    {
        if (placedMonster == null)
        {
            placedMonster = (GameObject)Instantiate(MonsterPrefab, transform.position, Quaternion.identity);
        }
        else if (CanUpgradeMonster())
        {
            placedMonster.GetComponent<MonsterData>().IncreaseLevel();
        }
    }

    private bool CanUpgradeMonster()
    {
        if (placedMonster != null)
        {
            MonsterData monsterData = placedMonster.GetComponent<MonsterData>();
            MonsterLevel nextLevel = monsterData.GetNextLevel();
            if (nextLevel != null)
            {
                return true;
            }
        }
        return false;
    }
}

```

</details>
