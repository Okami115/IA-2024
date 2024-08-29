using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Vector2IntGrapf<NodeType> 
    where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
{ 
    public List<NodeType> nodes = new List<NodeType>();

    private TypeOfPathFinder typeOfPathFinder;

    public Vector2IntGrapf(int x, int y, TypeOfPathFinder typeOfPathFinder) 
    {
        this.typeOfPathFinder = typeOfPathFinder;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new UnityEngine.Vector2Int(i, j));
                node.SetDistanceMethod((Vector2Int other) => { return (int)Vector2Int.Distance(node.GetCoordinate(), other); });
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
                Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1)
                currentNode.AddNeighbor(neighbor);

            else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                currentNode.AddNeighbor(neighbor);

            if(typeOfPathFinder == TypeOfPathFinder.DijstraPathfinder || typeOfPathFinder == TypeOfPathFinder.AStarPathfinder)
            {
                if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                    currentNode.AddNeighbor(neighbor);
            }
        }
    }

}
