using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;

    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;

    private Pathfinder<Node<Vector2Int>, Vector2Int> pathfinder;
    private ICollection<Node<Vector2Int>> path;

    private void OnEnable()
    {
        grapfView.ChangePathFinder += StartPathFinder;
    }

    private void OnDestroy()
    {
        grapfView.ChangePathFinder -= StartPathFinder;
    }

    void StartPathFinder()
    {
        StopAllCoroutines();
        startNode = grapfView.grapf.nodes[UnityEngine.Random.Range(0, grapfView.grapf.nodes.Count)];
        destinationNode = grapfView.grapf.nodes[UnityEngine.Random.Range(0, grapfView.grapf.nodes.Count)];

        switch (grapfView.GetTypeOfPathFinder)
        {
            case TypeOfPathFinder.BreadthPathfinder:
                pathfinder = new BreadthPathfinder<Node<Vector2Int>, Vector2Int>();
                path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
                break;
            case TypeOfPathFinder.DepthFirstPathfinder:
                pathfinder = new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>();
                path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
                break;
            case TypeOfPathFinder.DijstraPathfinder:
                pathfinder = new DijstraPathfinder<Node<Vector2Int>, Vector2Int>();
                path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
                break;
            case TypeOfPathFinder.AStarPathfinder:
                pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
                path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
                break;
            default:
                break;
        }

        StartCoroutine(Move(path));
    }

    public IEnumerator Move(ICollection<Node<Vector2Int>> path) 
    {
        if (path == null)
            yield return null;

        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;


        foreach (Node<Vector2Int> node in grapfView.grapf.nodes)
        {
            if(node.Equals(startNode))
                Gizmos.color = Color.blue;
            else if(node.Equals(destinationNode))
                Gizmos.color = Color.black;
            else if(path.Contains(node))
                Gizmos.color = Color.yellow;
            else
                Gizmos.color = Color.white;

            Vector3 nodeCordinates = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            Gizmos.DrawWireSphere(nodeCordinates, 0.2f);
        }
    }
}
