using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public interface INode
{
    public bool IsBloqued();
    public List<Node> GetNeighbors();
    public bool isEqualsTo(INode other);
}

public interface INode<Coorninate> 
{
    public void SetCoordinate(Coorninate coordinateType);
    public Coorninate GetCoordinate();
}
