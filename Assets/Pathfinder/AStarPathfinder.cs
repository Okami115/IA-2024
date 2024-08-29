using System;
using System.Collections.Generic;

public class AStarPathfinder <Node, Coorninate> : Pathfinder<Node, Coorninate>
    where Node : INode<Coorninate>
    where Coorninate : IEquatable<Coorninate>
{
    protected override int Distance(Node A, Node B)
    {
        return A.GetDistance(B.GetCoordinate());
    }

    protected override ICollection<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        foreach (Node neighbor in node.GetNeighbors())
        {
            neighbors.Add(neighbor);
        }

        neighbors.Reverse();
        return neighbors;
    }

    protected override bool IsBloqued(Node node)
    {
        return node.GetIsBloqued();
    }

    protected override int MoveToNeighborCost(Node A, Node B)
    {
        return A.GetCostToNeighbors(B);
    }

    protected override bool NodesEquals(Node A, Node B)
    {
        return A.Equals(B);
    }
}
