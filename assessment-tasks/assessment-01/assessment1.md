# 01: Introduction to Unity Scripting Assessment Tasks

For this first assessment task, we are going to use a library called DoTween. It is incredibly easy, lightweight and powerful, and I personally use it in all of my Unity projets.

DoTween allows you to easily create Tweens in your code. A tween is just a change in value over time. They can be used for animation, complicated sequences of events, or running some code after a delay.

> Download DoTween: <https://dotween.demigiant.com/getstarted.php>

**Don't worry about step 3.**

I would recommend having a read of the documentation before proceeding. You don't have to memorize the whole library, but it's useful to see the structure for how tweens and sequences are used.

## Task 1: Sheep Feedback

In order to complete this task, your game must have the following feature:
- A feedback heart model appears whenever a sheep eats a hay bale.
- This model must appear at the position of the sheep that ate the hay
- The heart model must use tweens to do at least two of the following:
    - Move upwards
    - Scale up or down
    - Rotate
- The heart model must destroy itself after its feedback has been completed.

To get you started:
- Make sure you have your heart prefab from lesson 2 set up in your project.
- Add a new variable in your `Sheep` class: `public GameObject FeedbackHeart`
- While editing your Sheep prefab, assign your heart prefab to the `FeedbackHeart` field in the editor.
- In your `Sheep` class, instantiate the FeedbackHeart when the sheep eats a hay bale.
- Add a script to your feedback heart prefab that will perform your tweens when the heart is instantiated.

Look at the SheepHeartsExample video in the `assessment-1/Resources` folder for an example video of how this should look.

## Task 2: Sound Effects

**This task provides you with some code, then requires you to implement the remaining features yourself.**

### Setup

For a simple game like ours, it's easiest to have one Game Object handle all of our sound effects. This is because you run into complications if you are trying to play sound effects from objects such as the sheep, which can be destroyed when they are trying to play a sound.

>There are plugins such as FMOD that allow you to create much more dynamic and responsive audio landscapes for your games, but that is beyond the scope of this course.

We are going to play all of our sound effects from a Game Object called `SFXManager`. Start by creating it in the scene, and adding a new script with the following code:

```csharp
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public Transform Camera;
    public AudioClip ShootSFX;
    public AudioClip SheepHitSFX;
    public AudioClip SheepDropSFX;

    private void Awake()
    {
        Instance = this;
    }
}
```

<details>
<summary>What's up with this code?</summary>

You might notice that this code has a reference to a `SFXManager` inside its `SFXManager` class. What's up with that?

The `static` keyword tells C# that this variable belongs to the `SFXManager` **class**, and **not** any one instance. Our `Awake` function assigns our `SFXManager` instance that exists in our scene to that static variable. Confusing? A little. Just know that this will allow us to call this SFX Manager from anywhere else in the code, without having to set up a reference for it and assign it in editor.

This technique is a programming pattern called the **singleton** pattern. it's not always appropriate to use it, but if there's only ever going to be **one** of this particular class, that's usually a good time to use it.

It's OK if you don't understand it right away - you will quickly see how incredibly useful singletons can be. They'll save you many headaches. Trust me.

<span style="color:red">TLDR: Use the Singleton pattern if you are only ever going to have one of something, and you want to easily reference it.</span>
</details>


### Implementation

With this singleton set up, you need to make it so that the "Shoot" sound effect plays whenever the hay machine fires hay, the "SheepHit" sound effect plays when the hay bale hits the sheep, and the "SheepDropped" sound effect plays when a sheep falls off the map.

All of these Sound Effects can be found in your Assets folder.

- Tips:
    - Create 3 public functions in your SFX manager, one to play each sound effect.
    - From elsewhere in your code, you can reference your `SFXManager` with `SFXManager.Instance`.
    - Look at Unity's AudioSource in the documentation: <https://docs.unity3d.com/ScriptReference/AudioSource.html> There is a **static method** that may help you play an audio clip.
    - There is another way you can set this up using Unity's **Audio Source** component. You are welcome to attempt a solution using this method if you would prefer.

## Task 3: Scoring

This task requires you to add scoring to your game. It will require you to write some code and set up a User Interface in the editor.

### Setup / Part 1:

Create a new script called `GameManager` and start with the following code:

```csharp
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Lets us reference this instance from any other script.
    public static GameManager Instance;

    // A number tracking how many sheep we've saved.
    // [HideInInspector] tells Unity not to show this variable in the editor, even though it is public.
    [HideInInspector]
    public int sheepSaved; 

    // A number tracking how many sheep have been dropped.
    [HideInInspector]
    public int sheepDropped; 

    // A number specifying how many sheep can be dropped before we lose the game.
    public int sheepDroppedBeforeGameOver;

    // A reference to the sheep manager.
    public SheepManager sheepSpawner; 

    public void Awake () 
    {
        Instance = this;
    }

    public void SaveSheep()
    {
        // TODO => Increase the number of sheep saved by 1.
    }

    private void GameOver()
    {
        // TODO => Destroy all the sheep on the map and stop the sheep manager from
        // spawning more sheep.
    }

    public void DroppedSheep()
    {
        // TODO => Record that we have dropped a sheep, 
        // then call GameOver() if we have dropped too many.
    }

}
```
Where there is a comment saying TODO, you must implement that part of the function by yourself.

Once you have implemented your functions, call them from the relevant parts of your code.

> Tip: You can test that things are working by writing `Debug.Log()` statements in your `GameManager` functions. You can log messages to Unity's console with this function, and you can log out variables to see what their value is at a specific point in time. It could be useful to log out `sheepSaved` in your `SaveSheep` function.
> Resource: <https://docs.unity3d.com/ScriptReference/Debug.Log.html>

### Part 2:

Now that you have a game manager to keep track of your player's score, you will need to display it on the screen.

Part 2 of this task is to create a User Interface for your game that will display the player's score (how many sheep they have fed) and how many lives they have remaining (once this number reaches 0, we lose).

> A User Interface in Unity is a 2D space in which we can place useful graphics and interactables that inform the player about crucial game state or allow them to quickly perform important actions. You will have seen user interfaces in almost every game you've played. They can be something as simple as a text box, to an in-depth Skyrim progression tree.

To get started, you may need to install the Text Mesh Pro package. Go to Window -> Package Manager and search for "Text Mesh Pro" with **Packages: Unity Registry** selected. Install the package into your project. Text Mesh Pro allows you to easily create great looking UI text, and is built in to later Unity versions.

Tips:
- You will need to use the following components:
    - `Canvas`
    - `Image`
    - `Text - Text Mesh Pro`
- You will need a script for your User Interface that reads the score from the game manager and displays it by changing the value of a text field or visibility of an image.

> Resource: <https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/class-Canvas.html>

Look at the HUDExample video in the `assessment-1/Resources` folder for an example video of how this should look.


## Task 4: Scene Management

There is already a title screen set up in the project, but it doesn't do anything yet. We are going to replace the 3D objects there with a User Interface. You can make a menu with 3D objects, but unless you are doing this for stylistic reasons, a User Interface is much better.

- Our User Interface will have "Start" and "Quit" buttons. You will need to use the `Button - Text Mesh Pro` component.
- We will need to add a script to our Start button that loads the Game scene. We can load scenes using Unity Engine's build in `SceneManager` class:
- Resource: <https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.html>
- We will need to add a script to our Quit button that quits the game when it is clicked.

To complete this task, you must make your game load in to the **game** scene when the **Start** button is clicked in the **Title** scene, and quit when the **Quit** button is clicked. In editor, clicking the quit button should stop play mode. Your main game scene game must also **return to the title screen** once the player runs out of lives.