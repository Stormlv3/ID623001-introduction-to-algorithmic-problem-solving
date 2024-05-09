# 06: Pathfinding

We now have all of the building blocks we need to get our pathfinding algorithm working. Today we will implement the recursive algorithm that will perform all of the pathfinding logic.

## Recursion

**Recursion** is a programming concept whereby a problem is defined in terms of itself. Practically, **recursion** manifests as a function that calls itself until some condition is met. In order for an algorithm to be recursive, the following properties must apply to it:

- The algorithm has a base case (a way of stopping)
- The algorithm works towards the base case (by breaking down the problem into a simpler step)
- The algorithm calls itself

How does this apply to our pathfinding?

### Recursion in pathfinding

We can think about the problem of finding a path in these terms:

- If we are at the end node, return it
- If we are not at the end node, take a step towards the end node
- Do this again

Framing our pathfinding problem in these terms allows us to consider it as a recursive algorithm.

- _If we are at the end node, return it_ **<- Base Case**
- _If we are not at the end node, take a step towards the end node_ **<- Break down the problem**
- _Do this again_ **<- Algorithm calls itself**

## Today's Task

In your `Pathfinder.cs` script, create a method called `FindPath`. Use the following code as a starting point:

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
         
        while (openList.Count > 0) 
        {
            // Consider the lowest fCost node.
            Node currentNode = GetLowestFCostNode(openList);
            
            // TODO: implement this loop. I'll break it into conceptual blocks for you to work out:

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

<details>

<summary>Solution Example</summary>

```csharp
public List<Node> FindPath(int startX, int startY, int endX, int endY)
{
    Node startNode = graph[startX,startY];
    Node endNode = graph[endX, endY];

    List<Node> openList = new List<Node> { startNode };
    List<Node> closedList = new List<Node>();

    int graphWidth = graph.GetLength(0);
    int graphHeight = graph.GetLength(1);

    for(int x = 0; x < graphWidth; x++)
        for(int y = 0; y < graphHeight; y++)
        {
            Node pathNode = graph[x, y];
            pathNode.gCost = int.MaxValue;
            pathNode.CalculateFCost();
            pathNode.cameFromNode = null;
        }

    startNode.gCost = 0;
    startNode.hCost = CalculateDistanceCost(startNode, endNode);
    startNode.CalculateFCost();

    while(openList.Count > 0)
    {
        Node currentNode = GetLowestFCostNode(openList);
        if(currentNode == endNode)
            return CalculatePath(endNode);

        openList.Remove(currentNode);
        closedList.Add(currentNode);

        foreach(Node neighbourNode in GetNeighbourList(currentNode)){
            if(closedList.Contains(neighbourNode)) continue;

            if(!neighbourNode.isWalkable){
                closedList.Add(neighbourNode);
                continue;
            }

            int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
            if(tentativeGCost < neighbourNode.gCost){
                neighbourNode.cameFromNode = currentNode;
                neighbourNode.gCost = tentativeGCost;
                neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                neighbourNode.CalculateFCost();

                if(!openList.Contains(neighbourNode))
                    openList.Add(neighbourNode);
            }
        }
    }

    //out of nodes on the open list
    return null;
}
```

</details>




