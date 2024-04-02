# 02: Game Mechanics Assessment Tasks

For this assessment task, we will make our towers shoot at the enemies, and be able to defeat them.

## Enemy Health Bars

Being able to see a visual representation of how close an enemy is to being defeated is important for this type of game. It gives the player a sense of satisfaction to see enemies take damage, and provides information about how tough different types of enemies are. A health bar is not necessarily the only way to do this (for example, Bloons Tower Defence uses different coloured Balloon layers), but it is generally the most straight-forward.

There are different methods for creating health bars, such as overlaying UI elements onto the scene, but we are going to use the simplest method, which is by using world-space game objects.

Setup:
- Using the assets **Images\Objects\HealthBarBackground** and **Images\Objects\HealthBar**, create a health bar object that is childed to the **Enemy** prefab.
- The **background** should be a direct child of the **enemy.**
- The **health bar** itself should be a child of the background.
- You may need to reposition the health bar background to have it display above the enemy.
- You will need to adjust the **x scale** and **position** of the health bar object to make it stretch across the length of the background.
- Add a `SortingGroup` component to the health bar background. This will group all child `SpriteRenderer`s together and make them display in the order that they appear in the hierarchy. This will ensure that your health bar object always appears in front of the background.

> Check the **resources** folder for an example of how the final health bar should look.

Once your health bar object is set up, create a new script called `EnemyHealth.cs` and attach it to the health bar (**not the health bar background**).

You will be provided with some starter code here - the task requirement is to **finish implementing the code** to achieve the required functionality.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour 
{
    public float maxHealth = 100;
    public float currentHealth = 100;
    private float originalXScale;

    private void Start() 
    {
        // The health bar takes note of its initial x scale, so that it can
        // rescale itself relative to that initial scale.
        originalXScale = gameObject.transform.localScale.x;
    }

    private void Update() 
    {
        // newScale is going to be what we set the scale to. Initially, it's
        // just whatever the current scale is.
        Vector3 newScale = gameObject.transform.localScale;
        
        // TODO: Update the x value of newScale. The new value should be a number
        // between 0 and originalXScale based on our currentHealth and maxHealth
        // I.E. if currentHealth is 0, x scale should be 0. If currentHealth
        // == maxHealth, then x scale should be originalXScale.

        gameObject.transform.localScale = newScale;
    }
}

```

> Tip: A good way to approach this problem is to first try to calculate a number between 0 and 1 that represents the current portion of health remaining. We can then multiply that number with our original scale to get the new scale.

You can test that your code is working by changing the default `currentHealth` value for the enemies. If you set `currentHealth` to `50`, the health bar should be half-full. If you set it to `10`, the health bar should be almost depleted.

## Bullets

Our towers are going to fire bullets at the enemies to reduce their health. Once an enemy's health reaches 0, it is destroyed!

First we will create the bullet logic, then we will have our towers fire the bullets.

Setup:
- Drag and drop **Images/Objects/Bullet1** into your scene to create a bullet game object. Make it a prefab.
- Add a `CircleCollider2D` to the bullet prefab and make it a trigger.
- Add a new script to the bullet prefab: `Bullet.cs`
- Your `Enemy` prefab will require a `Rigidbody2D` and a collider. Consider which type of `Rigidbody2D` is appropriate in this case (dynamic vs kinematic)
- The `Enemy` prefab should also be given the "Enemy" tag. (if this tag doesn't exist in your project, create it manually and **make sure it's assigned to the enemy**).

You will need to create your bullet script **from scratch.** The script should do the following:

- Define **damage**, **speed** and a **target** for the bullet.
- Make the bullet **move towards the target over time** based on its **speed** (`Vector2.MoveTowards` may come in handy).
- The bullet's target could be destroyed by another bullet while it is traveling. If the target no longer exists, this bullet should destroy itself.
- When the bullet's trigger enters another collider, check to see if that collider was an enemy. If it was, resolve the collision with the enemy.
- When resolving a collision with an enemy, the bullet should find a reference to the enemy's **health** (`GetComponentInChildren` could be useful).
- The bullet should **reduce the enemy's current health** by its **damage** value.
- If this damage reduces the enemy's health to **0 or lower**, the enemy should be **destroyed**. Award some `Gold` to the player via the `GameManager`.
- Regardless of whether or not the enemy was destroyed, the bullet should then be destroyed.


You can test if your code is working by placing an enemy and a bullet in the scene by default, and setting the bullet's target to the enemy. If the bullet flies towards the enemy and damages it, like in the demonstration video, you have implemented the bullet feature correctly.

> Resource: <https://docs.unity3d.com/ScriptReference/Vector2.MoveTowards.html>

> Resource: <https://docs.unity3d.com/ScriptReference/Component.GetComponentInChildren.html>

> Resource: <https://docs.unity3d.com/ScriptReference/Object.Destroy.html>

## Targeting Enemies

Our towers need to be able to identify enemies within range and attack them.

Setup:
- Your `Monster` prefab should use a `CircleCollider2D` that is a trigger to define its firing range.
- The `Monster` prefab should have its physics layer set to **Ignore Raycast**. This will ensure the collider doesn't block our clicks that are intended for the `MonsterSlot` beneath it. 

Create a new script and call it `EnemyShooter.cs`. Build upon the following starter code:

**Where there is a comment marked "TODO", you must implement that functionality yourself.**

```csharp
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour 
{
    public float shotCooldown;
    private float lastShotTime;
    private List<GameObject> enemiesInRange = new List<GameObject>();

    public void Update() 
    {
        if (Time.time - lastShotTime >= shotCooldown) 
        {
            // TODO: Our shot is off cooldown. If there is an enemy in range,
            // shoot at the first available one.
        }
    }

    public void Shoot(GameObject target) 
    {
        // TODO: Instantiate a bullet, set its target to the enemy we want to shoot.
        // Make sure we update our lastShotTime to Time.time (see resource below).
    }

    // TODO: When an object with the "Enemy" tag enters EnemyShooter's trigger, it should add that enemy's GameObject to its list of enemies who are in range.

    // TODO: When an object with the "Enemy" tag leaves EnemyShooter's trigger, it should remove that enemy's GameObject from its list of enemies who are in range.

}

```

If you have implemented this step successfully, your towers should **fire bullets** at the enemies **as they enter into range** of the tower. A tower should **stop** firing at an enemy **if that enemy is no longer within range**.

> Resource: <https://docs.unity3d.com/ScriptReference/Time-time.html>

Check the resources folder for an example video of how this should look.


## Bullet Tiers

Upgrading our towers (monsters) currently doesn't make them any stronger, which is a bit disappointing. Ideally, each tier of tower should shoot a **more powerful bullet** than the one before it.

To complete this task:

- You should have **3 different bullet prefabs**, each using a different bullet sprite and dealing successively higher amounts of damage.
- Each monster level should fire a **different bullet** (monster 0 should fire the weakest bullet, monster 2 should fire the strongest).
- Each monster level should fire at a **quicker rate.**

> Tip: We already define the monster levels in our `MonsterData` script. We could define the bullet and fire rate for each level here as well.

## Advanced Features

Implement these advanced features. These tasks are **self-directed** and will require some **independent research** and **problem-solving.**

- Have towers **rotate** to face the enemy they are targeting.
- Have towers **prioritize** the enemy **closest** to the cookie (remember, this is how far the enemy is along the path, not direct distance between the enemy and the cookie).
- Play **sound effects** for the following in-game occurences:
    - A tower is placed
    - A tower fires a bullet
    - An enemy is destroyed
    - A life is lost
- Implement **Enemy 2**. Make this enemy faster and tougher than the default enemy, and have them spawn at later waves.