using System;
using System.Collections.Generic;

public abstract class Pathfinder<NodeType, Coorninate>
    where NodeType : INode<Coorninate>
    where Coorninate : IEquatable<Coorninate>
{
    public ICollection<NodeType> FindPath(NodeType startNode, NodeType destinationNode, ICollection<NodeType> graph)
    {
        Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, int Heuristic)> nodes =
            new Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, int Heuristic)>();

        foreach (NodeType node in graph)
        {
            nodes.Add(node, (default, 0, 0));
        }

        List<NodeType> openICollection = new List<NodeType>();
        List<NodeType> closedICollection = new List<NodeType>();

        openICollection.Add(startNode);

        while (openICollection.Count > 0)
        {
            NodeType currentNode = openICollection[0];
            int currentIndex = 0;

            for (int i = 1; i < openICollection.Count; i++)
            {
                if (nodes[openICollection[i]].AcumulativeCost + nodes[openICollection[i]].Heuristic <
                nodes[currentNode].AcumulativeCost + nodes[currentNode].Heuristic)
                {
                    currentNode = openICollection[i];
                    currentIndex = i;
                }
            }

            openICollection.RemoveAt(currentIndex);
            closedICollection.Add(currentNode);

            if (NodesEquals(currentNode, destinationNode))
            {
                return GeneratePath(startNode, destinationNode);
            }

            foreach (NodeType neighbor in GetNeighbors(currentNode))
            {
                if (!nodes.ContainsKey(neighbor) ||
                IsBloqued(neighbor) ||
                closedICollection.Contains(neighbor))
                {
                    continue;
                }

                int tentativeNewAcumulatedCost = 0;
                tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, neighbor);

                if (!openICollection.Contains(neighbor) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                {
                    nodes[neighbor] = (currentNode, tentativeNewAcumulatedCost, Distance(neighbor, destinationNode));

                    if (!openICollection.Contains(neighbor))
                    {
                        openICollection.Add(neighbor);
                    }
                }
            }
        }

        return null;

        ICollection<NodeType> GeneratePath(NodeType startNode, NodeType goalNode)
        {
            List<NodeType> path = new List<NodeType>();
            NodeType currentNode = goalNode;

            while (!NodesEquals(currentNode, startNode))
            {
                path.Add(currentNode);
                currentNode = nodes[currentNode].Parent;
            }

            path.Add(startNode);
            path.Reverse();
            return path;
        }
    }

    protected abstract ICollection<NodeType> GetNeighbors(NodeType node);

    protected abstract int Distance(NodeType A, NodeType B);

    protected abstract bool NodesEquals(NodeType A, NodeType B);

    protected abstract int MoveToNeighborCost(NodeType A, NodeType b);

    protected abstract bool IsBloqued(NodeType node);
}