using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoveCaravanaState : State
{
    private Pathfinder<Node<Vector2Int>, Vector2Int> pathfinder;
    private List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

    private GrapfView grapfView;
    private bool isReturnHome;
    private bool hasMine = false;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector2Int> currentNode = parameters[0] as Node<Vector2Int>;
        grapfView = parameters[1] as GrapfView;
        Node<Vector2Int> destinationNode = new Node<Vector2Int>();

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            hasMine = false;

            foreach (Mine<Vector2Int> mine in grapfView.mines)
            {
                if (mine.currentFood <= 0 && mine.miners.Count > 0)
                {
                    destinationNode = mine.currentNode;
                    hasMine = true;
                    break;
                }
            }

            pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

            if (grapfView.mines.Count == 0 || grapfView.isAlert || !hasMine)
            {
                path = pathfinder.FindPath(currentNode, grapfView.urbanCenter, grapfView.grapf.nodes) as List<Node<Vector2Int>>;
                    isReturnHome = false;

                if (grapfView.isAlert)
                    isReturnHome = true;
            }
            else
            {
                path = pathfinder.FindPath(currentNode, destinationNode, grapfView.grapf.nodes) as List<Node<Vector2Int>>;
                isReturnHome = false;
            }



        });

        return result;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        Transform transform = parameters[0] as Transform;
        float speed = Convert.ToSingle(parameters[1]);
        Caravana caravana = parameters[2] as Caravana;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {
            if (path.Count > 0)
            {
                Vector3 aux = new Vector3(grapfView.OffsetPublic * path[0].GetCoordinate().x, grapfView.OffsetPublic * path[0].GetCoordinate().y);

                transform.position += (aux - transform.position).normalized * speed * Time.deltaTime;

                float dist = Vector3.Distance(transform.position, aux);
                float minDist = 0.1f;

                if (dist < minDist)
                {
                    caravana.currentNode = path[0];
                    path.Remove(path[0]);
                }
            }

        });

        result.SetTransition(() =>
        {

            if (grapfView.isAlert && !isReturnHome)
            {
                caravana.targetNode = grapfView.urbanCenter;
                OnFlag?.Invoke(Flags.IsAlert);
            }

            if (!grapfView.isAlert && isReturnHome)
            {
                OnFlag?.Invoke(Flags.IsAlert);
            }

            if (path.Count == 0)
            {
                if (caravana.currentNode != grapfView.urbanCenter)
                {
                    OnFlag?.Invoke(Flags.OnReadyToGiveFood);
                }
                else if (caravana.currentNode == grapfView.urbanCenter)
                {
                    OnFlag?.Invoke(Flags.OnWaitingOrders);
                }
            }

        });
        return result;
    }
}

