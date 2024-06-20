/// 
/// Lucas Storm
/// June 2024
/// Bugs: None known at this time.
/// 
/// This script handles the nodes for the pathfinding script.

public class Node
{
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public Node prevNode;

    public bool isWalkable;

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        hCost = 0;
        this.isWalkable = isWalkable;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}