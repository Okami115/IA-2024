using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class Node<Coordinate> : INode, INode<Coordinate>
{
    private Coordinate coordinate;
    private List<Node> Neighbors;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public bool IsBloqued()
    {
        return false;
    }

    public List<Node> GetNeighbors()
    {
        return Neighbors;
    }

    public bool isEqualsTo(INode other)
    {
        return coordinate == new Node<Coordinate>().coordinate;
    }
}