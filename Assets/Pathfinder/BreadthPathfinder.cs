using System;
using System.Collections.Generic;

public class BreadthPathfinder<Node, Coorninate> : Pathfinder<Node, Coorninate> 
    where Node : INode<Coorninate>
    where Coorninate : IEquatable<Coorninate>
{
    protected override int Distance(Node A, Node B)
    {
        return 0;
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
        return 0;
    }

    protected override bool NodesEquals(Node A, Node B)
    {
        return A.Equals(B);
    }
}
