using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public interface INode
{
    public bool GetIsBloqued();
    public void SetIsBloqued(bool isBloqued);
}

public interface INode<Coorninate> : INode
    where Coorninate : IEquatable<Coorninate>
{
    public void SetCoordinate(Coorninate coordinateType);

    public Coorninate GetCoordinate();

    public ICollection<INode<Coorninate>> GetNeighbors();

    public void AddNeighbor(INode<Coorninate> neighbor, int cost = 0);

    public void SetNeighborCost(INode<Coorninate> neighbor, int cost);

    public int GetCostToNeighbors(INode node);

    public int GetDistance(Coorninate coorninate);

    void SetDistanceMethod(Func<Coorninate, int> DistanceTo);

}
