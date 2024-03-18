# 04: Introduction to Unity Scripting - Managing Objects 


## Data structures

There are many different data structures in C#, and they all have their uses. None is better than the other, but some data structures are better suited for specific tasks than others. Knowing which data structures you should use for a given problem takes practice and experience.

The only data structure we will use today is called a `List`.

### Arrays

If you have programmed before, you will likely be familiar with Arrays. An array is a list of values ordered by index.

- An array can be any length
- Array indices start at 0. So the first value in the array would be at index 0, then the second value would be at index 1 etc.

If you need a refresher, check this resource:

> Resource: <https://www.w3schools.com/cs/cs_arrays.php>

### Lists

A list is just like an array, only much more convenient! C# Lists provide us with a bunch of handy methods for manipulating the stored values in commonly-needed ways:

- Adding elements to the end of the list - no need to know its length.
- Removing elements from the list and having it automatically reorder.
- Querying the list to see if it contains a specific value.

Code Example:

```csharp

void PlayWithList() {
    List<string> pokemon = new List<string> 
    {
        "Squirtle",
        "Charizard",
        "Pikachu",
        "Eevee",
        "Ho-oh"
    };

    // Adds a new element to the list:
    pokemon.Add("Rapidash");
    // The list will now be: ["Squirtle", "Charizard", "Pikachu", "Eevee", "Ho-oh", "Rapidash"]

    // Removes an existing element from the list and reorders it:
    pokemon.Remove("Squirtle");
    // The list will now be: ["Charizard", "Pikachu", "Eevee", "Ho-oh", "Rapidash"]

    // This condition evaluates to true, because "Pikachu" is in the List:
    if (pokemon.Contains("Pikachu")) 
    {
        Debug.Log("Gotta catch em' all");
    }
    

}

```

## Events

Unity’s Event System is very powerful and extremely useful for keeping your code clean. It allows for multiple classes to respond differently to a single in-game occurence. It also reduces dependency in your codebase; objects don't rely on each other as much, resulting in less "spaghetti code".

I like to think of an event as a worker on a factory line shouting out a status update. It would go something like this:

"Hey! I found a dead rat in this chocolate bar!"

The worker just shouts this out and keeps working. It's up to his superiors what they want to do with this message.

A higher-up in the factory, maybe the manager, could hear this message and decide "Right. There's a dead rat in the chocolate. I'm going to dump the whole batch."

The worker is not part of the decision to dump the batch, and doesn't know what his manager did with his callout (or even that his manager listened). All he knows is that when he comes into work the next day, the chocolate is a different brand.

Entities in your game are like the factory worker and the manager. Each entity should have a certain set of responsibilities that don't encroach on the responsibilities of the others. In programming terms, this is called **separation of concerns.**

So, how does this work in Unity?

Here is an example script showing two classes. The `FactoryWorker` class declares an event, which it calls from a function, and the `FactoryManager` class listens to that event, then reacts to it accordingly. Notice how the `FactoryWorker` doesn't have any references to the `FactoryManager`:

```csharp
using UnityEngine.EventSystem;

public class FactoryWorker : MonoBehaviour {
    // Declare the event.
    public UnityEvent OnRatFoundInChocolate = new UnityEvent();

    // Call this function when you find a dead rat.
    public void DiscoverRatInChocolate() 
    {
        // Hey, everyone. I found a dead rat!
        OnRatFoundInChocolate?.Invoke();
    }
}

public class FactoryManager : MonoBehaviour {
    // The manager is keeping an eye on the worker.
    private FactoryWorker factoryWorker;
    
    public void Start() 
    {
        // Set up a listener for when the worker discovers the dead rat.
        factoryWorker.OnRatFoundInChocolate.AddListener(DumpTheBatch)
    }

    public void DumpTheBatch() 
    {
        // DUMP THE BATCH!
    }
}
```

## Defining our own Unity Events

`UnityEvent` is great, but sometimes we need to provide **additional information** when we fire an event. We can define our **own versions** of Unity Events that take in specific **parameters**, or additional pieces of information that can add context to that event. 

In our chocolate factory worker example, using a default Unity Event would translate to something like this:

> "I found a dead rat in the chocolate".

If we added a parameter to the event to specify which conveyor belt we found the dead rat on, then it would read something like:

> "I found a dead rat in the chocolate on conveyer belt #7".

Alright, enough about dead rats.

As the developers, we get to decide whatever additional pieces of information will help the listeners of an event to know what to do with it.

How to do this in our code:

```csharp
public class Sheep : MonoBehaviour {
    // Defines our own special type of event.
    // We are calling it SheepEvent, and it takes a parameter of type Sheep.
    public class SheepEvent : UnityEvent<Sheep> { }

    // Both of these events are of type SheepEvent, so when we call them we have to provide a sheep instance.
    public SheepEvent OnAteHay = new SheepEvent();
    public SheepEvent OnDropped = new SheepEvent();

    public void EatHay() {
        // When we fire the event, we provide 'this' as a parameter. In C#, the 'this' keyword refers to 
        // the current instance of the object (in this case it will be of type Sheep, because this is
        // happening in the Sheep script).
        OnAteHay?.Invoke(this); // The ? just means to fire the event if it has any listeners.
    }

    public void Drop() {
        OnDropped?.Invoke(this);
    }
}

public class SheepManager : MonoBehaviour {
    public Sheep SheepPrefab;

    // Other code here...

    public void SpawnSheep() {
        Sheep newSheep = Instantiate(SheepPrefab);
        newSheep.OnAteHay.AddListener(HandleSheepAteHay);
        newSheep.OnDropped.AddListener(HandleSheepDropped);
    }

    private void HandleSheepAteHay(Sheep sheep) {
        // The "sheep" variable will refer to the sheep who called this event!
        // We can now do whatever we want with this sheep.
    }

    private void HandleSheepDropped(Sheep sheep) {
        // The "sheep" variable will refer to the sheep who called this event!
        // We can now do whatever we want with this sheep.
    }
}
```

## Using lists and events to manage our game state

Here we will have a collaborative problem solving session, thinking about how we would go about making a sheep spawning system that keeps track of all the sheep in the game.


<details>
<summary>Sheep Spawner Solution</summary>

Set up a Game Object `Sheep Manager` and give it as many empty children as you want. These empty Game Objects will be used as spawn positions for the sheep.

Give `Sheep Spawner` a script along these lines:

**Note that this script will throw a compilation error** if you just copy + paste it. You will need to set up events in your `Sheep` script. The sheep should have an event for when it eats hay, and when it drops off the map. I will leave those up to you to implement :)


```csharp
public bool canSpawn = true; 

public Sheep sheepPrefab; 
public List<Transform> sheepSpawnPositions = new List<Transform>(); 
public float timeBetweenSpawns; 

private List<Sheep> sheepList = new List<Sheep>(); 

private void Start() 
{
    StartCoroutine(SpawnRoutine());
}

private void SpawnSheep()
{
    Vector3 randomPosition = sheepSpawnPositions[Random.Range(0, sheepSpawnPositions.Count)].position; 
    Sheep sheep = Instantiate(sheepPrefab, randomPosition, sheepPrefab.transform.rotation); 
    sheep.OnEatenHay.AddListener(HandleSheepEatenHay);
    sheep.OnDropped.AddListener(HandleSheepDropped);
    sheepList.Add(sheep);
}

private void HandleSheepEatenHay(Sheep sheep) {
    sheepList.Remove(sheep);
    // Later we could add some points here.
}

private void HandleSheepDropped(Sheep sheep) {
    sheepList.Remove(sheep);
    // later, we could subtract lives here.
}

private IEnumerator SpawnRoutine() 
{
    while (canSpawn) 
    {
        SpawnSheep(); 
        yield return new WaitForSeconds(timeBetweenSpawns); 
    }
}
```
In editor, assign each of your empty `GameObjects` that you set up to be spawn points to the `sheepSpawnPositions` array.
Assign the `Sheep` prefab from your project window **NOT FROM THE SCENE!**


</details>
