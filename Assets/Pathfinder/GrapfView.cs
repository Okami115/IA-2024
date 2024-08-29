using System;
using UnityEngine;

[Serializable]
public enum TypeOfPathFinder
{
    BreadthPathfinder,
    DepthFirstPathfinder,
    DijstraPathfinder,
    AStarPathfinder
}

public class GrapfView : MonoBehaviour
{
    public Vector2IntGrapf<Node<Vector2Int>> grapf;

    [SerializeField] private TypeOfPathFinder typeOfPathFinder;
    [SerializeField] public int MaxGridSize;

    public Action ChangePathFinder;

    public TypeOfPathFinder GetTypeOfPathFinder { get => typeOfPathFinder; }

    private void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(MaxGridSize, MaxGridSize, typeOfPathFinder);
        ChangePathFinder?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            grapf = new Vector2IntGrapf<Node<Vector2Int>>(MaxGridSize, MaxGridSize, typeOfPathFinder);
            ChangePathFinder?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node == null)
                return;

            Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            foreach (INode<Vector2Int> neighbor in node.GetNeighbors())
            {
                Gizmos.color = Color.white;
                Vector2Int neighborCordinates = neighbor.GetCoordinate();
                Gizmos.DrawLine(nodeCordinates, new Vector3(neighborCordinates.x, neighborCordinates.y));
            }
        }
    }
}
