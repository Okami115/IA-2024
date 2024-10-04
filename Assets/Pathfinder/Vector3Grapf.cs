using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Grapf<NodeType>
    where NodeType : INode<UnityEngine.Vector3>, INode, new()
{
    public List<NodeType> nodes = new List<NodeType>();

    private TypeOfPathFinder typeOfPathFinder;
    private int offset;

    public Vector3Grapf(int x, int y, int offset, TypeOfPathFinder typeOfPathFinder)
    {
        this.typeOfPathFinder = typeOfPathFinder;
        this.offset = offset;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new UnityEngine.Vector3(i, 0, j));
                node.SetDistanceMethod((Vector3 other) => { return (int)Vector3.Distance(node.GetCoordinate(), other); });
                nodes.Add(node);
            }
        }

        foreach (NodeType node in nodes)
        {
            AddNodeNeighbors(node);
        }
    }

    private void AddNodeNeighbors(NodeType currentNode)
    {
        foreach (NodeType neighbor in nodes)
        {
            if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
                Math.Abs(neighbor.GetCoordinate().z - currentNode.GetCoordinate().z) == 1)
                currentNode.AddNeighbor(neighbor);

            else if (neighbor.GetCoordinate().z == currentNode.GetCoordinate().z &&
                Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                currentNode.AddNeighbor(neighbor);

            if (typeOfPathFinder == TypeOfPathFinder.DijstraPathfinder || typeOfPathFinder == TypeOfPathFinder.AStarPathfinder)
            {
                if (Math.Abs(neighbor.GetCoordinate().z - currentNode.GetCoordinate().z) == 1 && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                    currentNode.AddNeighbor(neighbor);
            }
        }
    }

}
