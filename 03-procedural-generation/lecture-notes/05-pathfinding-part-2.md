# 05: Pathfinding

We have written the code to generate our weighted pathfinding graph from our maze data - now it's time to implement some utility functions that will help the A* algorithm.
Our A* pathfinding algorithm will run on the weighted graph we have generated, and calculate a path from one node to another. These nodes are used to represent cells in the maze, so once we have calculated a path between 2 nodes, we can translate that into a path through our worldspace maze.

## Calculating gCost and hCost

We've talked about how A* pathfinding acts based on cost values that it assigns to nodes. As a quick reminder:
- **gCost** is the estimated distance of a given node from the start node.
- **hCost** is the heuristic cost - the estimated distance from a given node to the end node.
- **fCost** is the sum of gCost and hCost. Lower fCost nodes are prioritized by the pathfinder.

_How do we actually calculate these values?_

Since our graph represents nodes in _Euclidean_ space (i.e. distances are the same as they would be in the real world - we've essentially got a 2D plane of nodes), and each node has a coordinate on that plane, we can very accurately calculate the distance of any given node to the end node.

## Task 1: Distance Calculations

To get you started on this task, create a new script `Pathfinder.cs` and assign it as a component to a new `Pathfinder` game object in the scene.

`Pathfinder` is going to be in charge of running our A* algorithm on the node graph, and returning paths that can be used by the monster to navigate the maze.

The first thing our pathfinder should be able to do is calculate distances between nodes. These distances will be used to determine gCost and hCost for nodes.

**The problem:**
- Given 2 nodes, `from` and `to`, we need to return an integer value that approximates the distance between the 2 nodes.

Your first instinct here may be to take the co-ordinates and calculate the distance between them using Pythagorus' theorem. This would give us the Euclidean distance between the two nodes (i.e. distance "as the crow flies") (<https://en.wikipedia.org/wiki/Euclidean_distance>). While this would give us a fairly good estimation, our graph doesn't just let us cut directly from one node to any other (imagine the monster just phasing through the walls directly towards the player). We have to move through a grid of discreet nodes, each with its own coordinate. All this to say, there is a more accurate solution for distance calculation.

If we want to find the shortest path between 2 points on a grid, there's a simple and elegant algorithm:

- Keep moving diagonally until we are aligned with the end node on either the x or y dimension (whichever is closer)
- Keep moving straight towards the end node

And that's it! Utilizing the following starter code, your distance calculation function should use this logic to return an integer value for the distance between 2 nodes.

### Starter code:

```csharp

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public MazeConstructor MazeConstructor;
    public Node[,] Graph {get {return MazeConstructor.Graph; }}

    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;

    public int CalculateDistance(Node from, Node to)
    {
        // TODO: return a number that is the sum of all the moves required to get from "from" to "to" (diagonal & straight)
    }
}

```

<details>
<summary>Solution Example</summary>

```csharp
public int CalculateDistance(Node from, Node to)
{
    // Get the x distance between the 2 nodes
    int xDistance = Mathf.Abs(from.x - to.x);

    // Get the y distance between the 2 nodes
    int yDistance = Mathf.Abs(from.y - to.y);

    //  How many "straight" moves we can do (i.e. if xDistance and yDistance are the same, we just move diagonally towards the point.)
    // The more they differ, the more straight moves we can perform.
    int remaining = Mathf.Abs(xDistance - yDistance);

    // Move diagonally to align with the closest coordinate.
    int diagonalCost = MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance);

    // All the non-diagonal moves are just straight moves towards the target.
    int straightCost =  MOVE_STRAIGHT_COST * remaining;

    // Return the sum of the cost of diagonal moves + the cost of straight moves.
    return diagonalCost + straightCost;
}
```

</details>

## Task 2: Getting Neighbours

Another thing the Pathfinder should be able to do is get a list of all the nodes that are neighbours to a given node. This will be the list of nodes that are added to the list of potentially viable nodes after that given node has been considered. The next task is to implement a method `GetNeighbourList` that takes a node as a parameter and returns a list of nodes that are all the nodes that are neighbours to it.

To consider:
- We have access to the pathfinding graph through the `Graph` accessor. Referring to this will be essential. Remember that each node has an x and y co-ordinate, so we can use these to determine proximity.
- A node may be at the edge of the graph. If this is the case, it won't have neighbours in the direction of the edge. Your code should account for this.

### Starter code:

```csharp

public List<Node> GetNeighbourList(Node node) 
{
    // TODO: Implement this function.
}

```

<details>

<summary>Solution Example</summary>

```csharp

private List<Node> GetNeighbourList(Node currentNode)
{
    List<Node> neighbourList = new List<Node>();

    // Only consider nodes to the left if we're not on the far left edge.
    if (currentNode.x - 1 >= 0)
    {
        neighbourList.Add(Graph[currentNode.x - 1, currentNode.y]);

        if (currentNode.y - 1 >= 0)
            neighbourList.Add(Graph[currentNode.x - 1, currentNode.y - 1]);
        if (currentNode.y + 1 < Graph.GetLength(1))
            neighbourList.Add(Graph[currentNode.x - 1, currentNode.y + 1]);
    }

    // Only consider nodes to the right if we're not on the far right edge.
    if (currentNode.x + 1 < Graph.GetLength(0))
    {
        neighbourList.Add(Graph[currentNode.x + 1, currentNode.y]);

        if (currentNode.y - 1 >= 0)
            neighbourList.Add(Graph[currentNode.x + 1, currentNode.y - 1]);
        if (currentNode.y + 1 < Graph.GetLength(1))
            neighbourList.Add(Graph[currentNode.x + 1, currentNode.y + 1]);
    }

    // Only add the below node if we're not on the bottom edge.
    if (currentNode.y - 1 >= 0)
        neighbourList.Add(Graph[currentNode.x, currentNode.y - 1]);
    // Only add the above node if we're not on the top edge.
    if (currentNode.y + 1 < Graph.GetLength(1))
        neighbourList.Add(Graph[currentNode.x, currentNode.y + 1]);

    return neighbourList;
}

```

</details>

## Conclusion

Now that we can estimate the distance between any 2 nodes, and get all the neighbouring nodes of a given node, we have the foundation to go ahead an implement the rest of the pathfinding algorithm in the next class.