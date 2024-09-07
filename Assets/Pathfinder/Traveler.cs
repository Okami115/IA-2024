using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;

    private Node<Vector2Int> startNode;
    private Node<Vector2Int> destinationNode;
    private Node<Vector2Int> currentNode;

    public int gold = 0;
    public float timeToExtract;

    private bool isRunning = false;

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

    void StartPathFinder(int startIndexNode, int endIndexNode)
    {
        isRunning = true;
        StopAllCoroutines();
        startNode = grapfView.urbanCenter;
        destinationNode = grapfView.mines[endIndexNode];

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
            currentNode = node;
            transform.position = new Vector3(grapfView.OffsetPublic * node.GetCoordinate().x, grapfView.OffsetPublic * node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }

        StartCoroutine(Extract());
    }

    public IEnumerator Extract()
    {
        while (gold < 15)
        {
            gold++;
            yield return new WaitForSeconds(timeToExtract);
        }

        StartCoroutine(ComeBack(path));
    }

    public IEnumerator ComeBack(ICollection<Node<Vector2Int>> path)
    {
        if (path == null)
            yield return null;

        List<Node<Vector2Int>> pathRerverse = path.ToList();
        pathRerverse.Reverse();

        foreach (Node<Vector2Int> node in pathRerverse)
        {
            currentNode = node;
            transform.position = new Vector3(grapfView.OffsetPublic * node.GetCoordinate().x, grapfView.OffsetPublic * node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
        transform.position = new Vector3(grapfView.OffsetPublic * grapfView.urbanCenter.GetCoordinate().x, grapfView.OffsetPublic * grapfView.urbanCenter.GetCoordinate().y);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isRunning)
            return;


        foreach (Node<Vector2Int> node in grapfView.grapf.nodes)
        {
            if(node.Equals(startNode))
                Gizmos.color = Color.blue;
            else if(node.Equals(destinationNode))
                Gizmos.color = Color.black;
            else if(path.Contains(node))
                Gizmos.color = Color.yellow;
            else if(grapfView.mines.Contains(node) && !node.Equals(destinationNode))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.white;

            Vector3 nodeCordinates = new Vector3(grapfView.OffsetPublic * node.GetCoordinate().x, grapfView.OffsetPublic * node.GetCoordinate().y);
            Gizmos.DrawWireSphere(nodeCordinates, 0.2f);
        }
    }
}
