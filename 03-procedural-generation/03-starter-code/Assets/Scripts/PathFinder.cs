/// 
/// Lucas Storm
/// June 2024
/// Bugs: Enemy often gets stuck in the pathfinding, typically at corners.
///       It also often moves through walls, meaning it passes over non-walkable
///       space which shouldn't be happening.
/// 
/// This script is responsible for the monster's movement throughout the maze.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public MazeConstructor MazeConstructor;
    public float MoveSpeed = 5f;
    public LineRenderer LR;
    public Node[,] Graph { get { return MazeConstructor.Graph; } } // Access to the graph from the MazeConstructor

    private Transform Player;
    private Transform Monster;

    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;

    // Calculate the distance between two nodes
    public int CalculateDistance(Node from, Node to)
    {
        int xDistance = Mathf.Abs(from.x - to.x);
        int yDistance = Mathf.Abs(from.y - to.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        int diagonalCost = MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance);
        int straightCost = MOVE_STRAIGHT_COST * remaining;
        return diagonalCost + straightCost;
    }

    // Get a list of walkable neighbor nodes for the current node
    private List<Node> GetNeighbourList(Node currentNode)
    {
        List<Node> neighbourList = new List<Node>();

        if (currentNode.x - 1 >= 0 && Graph[currentNode.x - 1, currentNode.y].isWalkable)
            neighbourList.Add(Graph[currentNode.x - 1, currentNode.y]); // Left

        if (currentNode.x + 1 < Graph.GetLength(0) && Graph[currentNode.x + 1, currentNode.y].isWalkable)
            neighbourList.Add(Graph[currentNode.x + 1, currentNode.y]); // Right

        if (currentNode.y - 1 >= 0 && Graph[currentNode.x, currentNode.y - 1].isWalkable)
            neighbourList.Add(Graph[currentNode.x, currentNode.y - 1]); // Down

        if (currentNode.y + 1 < Graph.GetLength(1) && Graph[currentNode.x, currentNode.y + 1].isWalkable)
            neighbourList.Add(Graph[currentNode.x, currentNode.y + 1]); // Up

        return neighbourList;
    }

    // Get the node with the lowest fCost from the list of nodes
    private Node GetLowestFCostNode(List<Node> nodeList)
    {
        Node lowestFCostNode = nodeList[0];
        for (int i = 1; i < nodeList.Count; i++)
        {
            if (nodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = nodeList[i];
            }
        }
        return lowestFCostNode;
    }

    // Calculate the path from the end node to the start node
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

    // Find the path from the start coordinates to the end coordinates
    public List<Node> FindPath(int startX, int startY, int endX, int endY)
    {
        Node startNode = Graph[startX, startY];
        Node endNode = Graph[endX, endY];

        List<Node> openList = new List<Node> { startNode };
        List<Node> closedList = new List<Node>();

        int graphWidth = Graph.GetLength(0);
        int graphHeight = Graph.GetLength(1);

        // Initialize all nodes
        for (int x = 0; x < graphWidth; x++)
        {
            for (int y = 0; y < graphHeight; y++)
            {
                Node pathNode = Graph[x, y];
                pathNode.gCost = int.MaxValue;
                pathNode.prevNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistance(startNode, endNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                return CalculatePath(endNode);
            }

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
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        return null;
    }

    private void Start()
    {
        StartCoroutine(WaitForObjects());
    }

    // Coroutine to wait until Player and Monster objects are found
    private IEnumerator WaitForObjects()
    {
        while (GameObject.FindGameObjectWithTag("Player") == null || GameObject.FindGameObjectWithTag("Monster") == null)
        {
            yield return null;
        }

        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Monster = GameObject.FindGameObjectWithTag("Monster").transform;
    }

    private void Update()
    {
        if (Player == null || Monster == null)
            return;

        // Calculate the path from monster to player
        List<Node> path = FindPath(
            Mathf.RoundToInt(Monster.position.x / MazeConstructor.GetComponent<MazeMeshGenerator>().width),
            Mathf.RoundToInt(Monster.position.z / MazeConstructor.GetComponent<MazeMeshGenerator>().width),
            Mathf.RoundToInt(Player.position.x / MazeConstructor.GetComponent<MazeMeshGenerator>().width),
            Mathf.RoundToInt(Player.position.z / MazeConstructor.GetComponent<MazeMeshGenerator>().width)
        );

        // Move monster along the calculated path
        if (path != null && path.Count > 1)
        {
            Vector3 nextPosition = new Vector3(path[1].y * MazeConstructor.GetComponent<MazeMeshGenerator>().width, Monster.position.y, path[1].x * MazeConstructor.GetComponent<MazeMeshGenerator>().width);
            Monster.position = Vector3.MoveTowards(Monster.position, nextPosition, MoveSpeed * Time.deltaTime);

            // Rotate monster to face the direction it's moving
            Vector3 direction = (nextPosition - Monster.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                Monster.rotation = Quaternion.Slerp(Monster.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }

        // Debug lines for visualizing the path
        if (path != null)
        {
            List<Vector3> points = new List<Vector3>();
            foreach (var node in path)
            {
                points.Add(new Vector3(node.y * MazeConstructor.GetComponent<MazeMeshGenerator>().width, 1f, node.x * MazeConstructor.GetComponent<MazeMeshGenerator>().width));
            }
            LR.positionCount = points.Count;
            LR.SetPositions(points.ToArray());
        }
    }
}
