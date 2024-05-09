# 06: Pathfinding

We now have all of the building blocks we need to get our pathfinding algorithm working. Today we will implement the algorithm that will perform all of the pathfinding logic.

## Algorithm concepts

Today's class is all about giving you a starting point from which to construct the most important part of the pathfinding algorithm. You have the entire class to focus on this one piece of code, so make the most of the time available, and give yourself up to 2 hours to work on the problem before looking at the solution example.

### Closed + Open Lists

A* stores 2 lists in working memory: `closedList` and `openList`. These are both lists of nodes and each is core to the functionality of A*.
- `closedList` holds all the nodes that have already been considered.
- `openList` holds all the nodes that are yet to be considered, but which we could potentially choose as our next node to consider.

Our algorithm will continuously update these lists based on the status of our search. If we run out of open nodes to consider, we couldn't find a path to the end node. If we find ourselves considering the end node, we can trace back the path we took to get there and return that as a path (ordered list of nodes).

## Today's Task

In your `Pathfinder.cs` script, implement the method called `FindPath`. Use the following code as a starting point:

```csharp
public class Pathfinder : MonoBehaviour 
{
    //... Methods & fields we implemented last class.

    public LineRenderer LR;

    // This Start method is purely for debugging and checking that your solution works.
    // It expects a LineRenderer component, which it will then use to draw a path
    // between the 2 nodes at the specified coordinates (in this example node (1, 1) and node (5,5)).
    private IEnumerator Start()
    {
        yield return null; // wait 1 frame to ensure that our graph has been generated.
        List<Node> path = FindPath(1, 1, 5, 5);
        List<Vector3> points = new List<Vector3>();
        if (path != null)
        {
            foreach(var node in path)
            {
                points.Add(new Vector3(node.y * mazeConstructor.MeshGenerator.width, 1f, node.x * mazeConstructor.MeshGenerator.width));
            }
            LR.positionCount = points.Count;
            LR.SetPositions(points.ToArray());
        }
    }

    // Utility function that returns the node with the lowest fCost from the provided list.
    private Node GetLowestFCostNode(List<Node> nodeList){
        Node lowestFCostNode = nodeList[0];
        for(int i = 1; i < nodeList.Count; i++)
            if(nodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = nodeList[i];

        return lowestFCostNode;
    }

    // Utility function that will be called once we have found the end node
    // to trace back the path that was taken to get there.
    private List<Node> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);
        Node currentNode = endNode;
        while (currentNode.prevNode != null)
        {
            path.Add(currentNode.prevNode);
            currentNode = currentNode.prevNode;
        }
        path.Reverse();
        return path;
    }

    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        Node startNode = graph[startX,startY];
        Node endNode = graph[endX, endY];

        // openList contains all the nodes we are currently considering.
        List<Node> openList = new List<Node> { startNode };
        // closedList is all the nodes we have already explored.
        List<Node> closedList = new List<Node>();

        // Do some graph initialization.
        int graphWidth = Graph.GetLength(0);
        int graphHeight = Graph.GetLength(1);

        for (int x = 0; x < graphWidth; x++)
            for (int y = 0; y < graphHeight; y++)
            {
                Node pathNode = Graph[x, y];
                // Until we've considered a node, make it's gCost massive.
                // Since gCost will be overwritten on a node if a cheaper path
                // is found to it.
                pathNode.gCost = int.MaxValue;
                // This will be set once we find the best node to get to this 
                // node from.
                pathNode.prevNode = null;
            }

        // Initialize the start node.
        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);
         
        // This is the heart of the algorithm:
        while (openList.Count > 0) 
        {
            // Consider the lowest fCost node.
            Node currentNode = GetLowestFCostNode(openList);

            // TODO: if the current node is the end node, return the path (use the CalculatePath method).

            // TODO: Take the current node out of the open list, and add it to the closed list.

            // TODO: Iterate over all of the current node's neighbours.
            // - If the node has already been considered (closed), do nothing.
            // - If the node isn't walkable, close it and do nothing.
            // - If the distance from current node to the neighbour is less than the neighbour's current gCost,
            // we have a potential node to explore. Set 3 fields on that neighbour: prevNode, gCost, hCost.
            // Add that neighbour to openList if it isn't already there.

            // TODO: If we get to the end of the while loop and our base condition hasn't been met, we couldn't
            // find a path. Return null.
        }
    }
}
```

<details>

<summary>Solution Example</summary>

```csharp
public List<Node> FindPath(int startX, int startY, int endX, int endY)
{
    Node startNode = Graph[startX, startY];
    Node endNode = Graph[endX, endY];

    List<Node> openList = new List<Node> { startNode };
    List<Node> closedList = new List<Node>();

    int graphWidth = Graph.GetLength(0);
    int graphHeight = Graph.GetLength(1);

    for (int x = 0; x < graphWidth; x++)
        for (int y = 0; y < graphHeight; y++)
        {
            Node pathNode = Graph[x, y];
            pathNode.gCost = int.MaxValue;
            pathNode.prevNode = null;
        }

    startNode.gCost = 0;
    startNode.hCost = CalculateDistance(startNode, endNode); 

    while (openList.Count > 0)
    {
        Node currentNode = GetLowestFCostNode(openList);
        if (currentNode == endNode)
            return CalculatePath(endNode);

        openList.Remove(currentNode);
        closedList.Add(currentNode);

        foreach (Node neighbourNode in GetNeighbourList(currentNode))
        {
            if (closedList.Contains(neighbourNode)) continue;

            if (!neighbourNode.isWalkable)
            {
                closedList.Add(neighbourNode);
                continue;
            }

            int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbourNode);
            if (tentativeGCost < neighbourNode.gCost)
            {
                neighbourNode.prevNode = currentNode;
                neighbourNode.gCost = tentativeGCost;
                neighbourNode.hCost = CalculateDistance(neighbourNode, endNode);

                if (!openList.Contains(neighbourNode))
                    openList.Add(neighbourNode);
            }
        }
    }
    return null;
}
```

</details>

## Summary

If implemented correctly, your Line Renderer should show a path through the maze between the nodes that you specified in the start method. Note that depending on which nodes you selected, there is the potential that no path is possible (if you chose a node which was closed.)

Next class we will be starting on the assessment tasks for assessment 3.


