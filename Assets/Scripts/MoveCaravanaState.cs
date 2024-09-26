using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoveCaravanaState : State
{
    private Pathfinder<Node<Vector2Int>, Vector2Int> pathfinder;
    private List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

    private GrapfView grapfView;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector2Int> currentNode = parameters[0] as Node<Vector2Int>;
        Node<Vector2Int> destinationNode = parameters[1] as Node<Vector2Int>;
        grapfView = parameters[2] as GrapfView;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
            path = pathfinder.FindPath(currentNode, destinationNode, grapfView.grapf.nodes) as List<Node<Vector2Int>>;
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

