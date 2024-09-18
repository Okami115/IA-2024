using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;

    private Node<Vector2Int> destinationNode;
    private Node<Vector2Int> currentNode;

    public float? gold = 0;

    Func<int> Gold;

    public float timeToExtract;
    public float speed;

    private bool isRunning = false;

    private FSM<Behaivours, Flags> fsm;

    private void OnEnable()
    {
        grapfView.ChangePathFinder += StartPathFinder;

        fsm = new FSM<Behaivours, Flags>();
    }

    private void OnDestroy()
    {
        grapfView.ChangePathFinder -= StartPathFinder;
    }

    void StartPathFinder(int startIndexNode, int endIndexNode)
    {
        isRunning = true;
        StopAllCoroutines();
        currentNode = grapfView.urbanCenter;
        destinationNode = grapfView.mines[endIndexNode];

        fsm.AddBehaviour<MoveState>(Behaivours.Move,
            onEnterParameters: () => { return new object[] { currentNode, destinationNode, grapfView }; }
            , onTickParameters: () => { return new object[] { transform, grapfView.OffsetPublic, speed }; });

        fsm.AddBehaviour<MiningState>(Behaivours.Mining, onTickParameters: () => { return new object[] { gold, timeToExtract, grapfView.minesWithGold[destinationNode] }; });

        fsm.SetTrasnsition(Behaivours.Move, Flags.OnReadyToMine, Behaivours.Mining, () => { Debug.Log("*Procede a minar*"); });

        Vector3 aux = new Vector3(grapfView.OffsetPublic * currentNode.GetCoordinate().x, grapfView.OffsetPublic * currentNode.GetCoordinate().y);

        transform.position = aux;

        fsm.ForceState(Behaivours.Move);
    }

    private void Update()
    {
        fsm.Tick();
    }

    public IEnumerator Extract()
    {
        while (gold < 15)
        {
            gold++;
            grapfView.minesWithGold[currentNode]--;
            yield return new WaitForSeconds(timeToExtract);
        }

        grapfView.minesWithGold.Remove(currentNode);
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
        /*
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
         */
    }
}
