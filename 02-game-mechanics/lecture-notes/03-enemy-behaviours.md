# 03: Game Mechanics - Enemy Behaviours

## Expanding Game Mechanics

We've got enemies navigating the path, but currently they just reach the end and do nothing. There is no lose state for the game, nor variety to the enemy spawns.

Today we will introduce a "lose" state by implementing a player health mechanic, whereby the player loses health when enemies reach the cookie. Once the player's health drops to 0, the game is lost.

We will also implement a wave system. A crucial mechanic in tower defense games is enemies increasing in power and quantity over time. Without this mechanic, players' defenses would reach a point where they were completely impermeable, and at this point there is no further gameplay to be had (other than watching the spectacle. Looks cool, but isn't much fun.) If our game has increasingly difficult enemy waves, there is much more gameplay and power progression to be explored.

## Freebie

Firstly, let's get the enemies facing in the direction they are moving. This isn't intended to be a focus for today, so the code will be provided for you:

(Add the provided lines to your enemy script, retaining the code that is already in there.)

```csharp
public class Enemy : MonoBehaviour
{
    private Vector3 lastPosition;
    [SerializeField] private GameObject body;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void Update() 
    {
        
        // -----------------------
        // Movement code goes here
        // -----------------------

        RotateIntoMoveDirection();
        lastPosition = transform.position;
    }

    private void RotateIntoMoveDirection()
    {
        Vector2 newDirection = (transform.position - lastPosition);
        body.transform.right = newDirection;
    }
}
   
```

<details>
<summary>Explanation</summary>

If you're curious as to how this works, we are keeping track of the position the enemy was in every frame (with `lastPosition = transform.position`). Then on each subsequent frame, before this line runs, we can compare `transform.position` to `lastPosition` to obtain a vector that represents how the enemy has moved between this frame and the last frame.

In `RotateIntoMoveDirection` we obtain that vector and store it in `newDirection`. We can then set `body.transform.right` to that new direction. Since the enemy sprite faces to the right, we are essentially redefining what `right` is for that transform, which rotates it to face the new direction. Vectors are cool!
</details>

## Task 1: Waves

It would be great if we could define enemy waves in the editor like we did with the tower levels. Let's start by adding this `Wave` class to `EnemySpawner.cs`:

```csharp
[System.Serializable]
public class Wave
{
    public Enemy enemyPrefab;
    public float spawnInterval = 2;
    public int maxEnemies = 20;
}
```

`Wave` is marked `Serializable`, which means it can be seen in the editor. If you don't do this, it won't show up!

In order to get waves working, we require the following:

- At least 2 defined waves in the editor

`EnemySpawner` should:
- Fire an event whenever a new wave starts (this will come into play later)
- Iterate over each wave that we've defined:
    - Spawn an enemy from this wave's `enemyPrefab`, one at a time, up to `maxEnemies` for this wave.
    - Wait for a predetermined amount of time before starting the next wave.

**We will do a collaborative problem solving session to implement the wave mechanics.**

<details>
<summary>Solution Example</summary>

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public Enemy enemyPrefab;
        public float spawnInterval = 2;
        public int maxEnemies = 20;
    }

    public Transform[] waypoints;
    public Wave[] waves; // The definition of all our waves (will be set in-editor)
    private int currentWaveIndex = 0;
    public int timeBetweenWaves = 5;

    // Events
    public UnityEvent OnWaveStarted = new UnityEvent();

    IEnumerator Start ()
    {
        // Current wave index will be incremented once we've spawned all the enemies for this wave.
        while (currentWaveIndex < waves.Length)
        {
            OnWaveStarted?.Invoke(); // Fire an event that we'll hook into later.
            for (var i = 0; i < waves[currentWaveIndex].maxEnemies; i++)
            {
                // Spawn an enemy, then wait for the spawn interval before continuing.
                SpawnEnemy();
                yield return new WaitForSeconds(waves[currentWaveIndex].spawnInterval);
            }
            // Increment the current wave index. You could also write a for loop rather than a while loop if you prefer.
            currentWaveIndex++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    public void SpawnEnemy()
    {
        // Looks at the current wave to determine which enemy we should spawn.
        Enemy newEnemy = Instantiate(waves[currentWaveIndex].enemyPrefab, transform.position, Quaternion.identity);
        newEnemy.waypoints = waypoints;
    }
}

Now 

```

</details>

Now, so long as you have properly defined your waves in-editor, you should see distinctive waves of enemies spawning and navigating through the map.

## Task 2: Wave Polish

It would be great if the user interface presented some more information about the wave state to the player.

There are some Game Objects already set up in our scene to help with this:

- `Canvas/WaveLabel`: A label that tells the player which wave they are currently on
- `Canvas/NextWaveTop/NextWaveTopLabel`: The top half of a graphic that will slide onto the screen to indicate the start of a wave.
- `Canvas/NextWaveBottom/NextWaveBottomLabel`: The bottom half of a graphic that will slide onto the screen to indicate the start of a wave.

In our `UserInterface.cs` script, let's implement the following features:

- Whenever a new wave starts (and at the start of the game), update the text of `WaveLabel` to reflect the current wave (e.g. if `EnemySpawner.currentWaveIndex` is 0, `WaveLabel` should read: "Wave: 1", etc.)
    - Your user interface will need a reference to the `EnemySpawner` (so that it can listen to the "wave started" event).
    - It will also need a reference to the `WaveLabel` text object.

- Whenever a new wave starts, play the slide in animation for both the top and bottom `NextWave` labels.

We haven't looked at animators yet. You can use this code to help you:

```csharp
topHalfWaveStartLabel.SetTrigger("nextWave");
bottomHalfWaveStartLabel.SetTrigger("nextWave"); 
```

These 2 lines will cause the wave start animations to play for each of the label halves. It's up to you to figure out where to put this code and set up the references to these 2 variables. (Hint: We've already got an event firing from `EnemySpawner` when a new wave starts.)

> Tip: `SetTrigger` is a public method of the `Animator` class. Your variables should be of that type if you want the above code to work.

<details>
<summary>Solution Example</summary>

```csharp
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public EnemySpawner EnemySpawner;
    [SerializeField] private Text goldLabel; 
    [SerializeField] private Text waveLabel;
    [SerializeField] private Animator topHalfWaveStartLabel;
    [SerializeField] private Animator bottomHalfWaveStartLabel;

    private void Awake()
    {
        GameManager.Instance.OnGoldSet.AddListener(HandleGoldSet);
        EnemySpawner.OnWaveStarted.AddListener(HandleWaveStarted);
    }

    private void HandleGoldSet()
    {
        goldLabel.text = "GOLD: " + GameManager.Instance.Gold.ToString();
    }

    private void HandleWaveStarted()
    {
        waveLabel.text = "WAVE: " + (EnemySpawner.CurrentWaveIndex + 1).ToString();
        // Fire off the animation for both label halves. When played at the same time, these
        // create a flashy effect.
        topHalfWaveStartLabel.SetTrigger("nextWave");
        bottomHalfWaveStartLabel.SetTrigger("nextWave");
    }
}
```

</details>

If set up correctly, we should see an accurate wave counter in the top-right of the screen, and a sweet "Wave Started" text animation every time a wave starts.


## Task 3: Health and Loss States

The Game Manager should track our player health and fire events both when the health is updated and when it reaches 0. Other parts of our game can then hook into these events and perform feedback when they happen.

- Update `GameManager.cs` to:
    - Store a private `health` variable.
    - Provide a public accessor for `health` that does the following in its setter:
        - Updates the value of `health`
        - Fires an event to say that health has been modified
        - Fires a different event when health reaches 0 (the game is lost)

<details>
<summary>Solution Example</summary>

Add this code into your `GameManager` script:

```csharp
public bool gameOver = false;
public UnityEvent OnHealthSet = new UnityEvent();
public UnityEvent OnGameOver = new UnityEvent();
private int health;
public int Health
{
    get { return health; }
    set
    {
        health = value;
        OnHealthSet?.Invoke();
        if (health <= 0 && !gameOver)
        {
            OnGameOver?.Invoke();
            gameOver = true;
        }
    }
}

```

</details>

Finally, implement the following features:

- Start the player with 5 health.
- Update the "Health" label in the top left corner of the screen whenever our health value is modified (This should be the job of our `UserInterface` script).
- Decrement (decrease by one) the player's health whenever an enemy reaches the cookie (you can use a trigger collider on the cookie, or make this happen when the enemy reaches their final waypoint).
- Play the Game Over animation when the player's health reaches 0 (there is an `Animator` attached to the `GameOverLabel` script. You'll need to call `.SetTrigger("gameOver")` on it to fire the animation. This is also the responsibility of `UserInterface`).

## Bonus Task

- The little green monsters hanging out on the cookie can be used as an additional representation of our lives. Make it so that whenever we lose a life, one of the green monsters is destroyed.

## Conclusion

We now have a solid foundation for expanding our game with harder waves and more powerful enemies. In your assessment tasks, you will be provided with some starter code which you will need to expand upon to get the player fighting back against the enemies. Finally, we will have our vengeance for all those eaten cookies!