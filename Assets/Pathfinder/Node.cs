using System;
using System.Collections.Generic;

public class Node<Coordinate> : INode<Coordinate>, IEquatable<Node<Coordinate>>
    where Coordinate : IEquatable<Coordinate>
{
    private Coordinate coordinate;
    private bool bloqued;
    private ICollection<INode<Coordinate>> neighbors = new List<INode<Coordinate>>();
    private Dictionary<INode, int> costToNeighbors = new Dictionary<INode, int>();
    private Func<Coordinate, int> DistanceTo;

    public void SetCoordinate(Coordinate coordinate)
    {
        this.coordinate = coordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public ICollection<INode<Coordinate>> GetNeighbors()
    {
        return neighbors;
    }

    ICollection<INode<Coordinate>> INode<Coordinate>.GetNeighbors()
    {
        return neighbors;
    }

    public bool GetIsBloqued()
    {
        return bloqued;
    }

    public int GetCostToNeighbors(INode node)
    {
        return costToNeighbors[node];
    }

    public void SetNeighborCost(INode<Coordinate> neighbor, int cost)
    {
        if(costToNeighbors.ContainsKey(neighbor))
            costToNeighbors[neighbor] = cost;
    }

    public void SetIsBloqued(bool isBloqued)
    {
        bloqued = isBloqued;
    }

    public void AddNeighbor(INode<Coordinate> neighbor, int cost = 0)
    {
        neighbors.Add(neighbor);

        costToNeighbors[neighbor] = cost;
    }

    public bool Equals(Node<Coordinate> other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<Coordinate>.Default.Equals(coordinate, other.coordinate) && bloqued == other.bloqued && Equals(neighbors, other.neighbors);
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Node<Coordinate>)obj);
    }

    public int GetDistance(Coordinate coorninate)
    {
        return DistanceTo(coorninate);
    }

    public void SetDistanceMethod(Func<Coordinate, int> DistanceTo)
    {
        this.DistanceTo = DistanceTo;
    }
}