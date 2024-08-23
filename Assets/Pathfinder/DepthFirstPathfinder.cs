using System.Collections.Generic;

public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    protected override int Distance(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        return (ICollection<NodeType>)node.GetNeighbors();
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        bool isEquals = true;

        if(A.GetType() != B.GetType())
            isEquals = false;

        if(A.IsBloqued() != B.IsBloqued())
            isEquals = false;

        if(A.GetHashCode() != B.GetHashCode())
            isEquals = false;

        return isEquals;
    }
}
