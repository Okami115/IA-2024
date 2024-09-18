using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoveState : State
{
    private Pathfinder<Node<Vector2Int>, Vector2Int> pathfinder;
    private List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

    Node<Vector2Int> currentNode;
    Node<Vector2Int> destinationNode;

    public GrapfView grapfView;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        BehaivioursAction result = new BehaivioursAction();

        currentNode = parameters[0] as Node<Vector2Int>;
        destinationNode = parameters[1] as Node<Vector2Int>;
        grapfView = parameters[2] as GrapfView;

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
        int OffsetPublic = Convert.ToInt32(parameters[1]);
        float speed = Convert.ToSingle(parameters[2]);

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {
            if(path.Count > 0)
            {
                Debug.Log("Permiso dijo el petiso...");
                Vector3 aux = new Vector3(OffsetPublic * path[0].GetCoordinate().x, OffsetPublic * path[0].GetCoordinate().y);

                transform.position += (aux - transform.position).normalized * speed * Time.deltaTime;

                float testX = aux.x - transform.position.x;
                float testY = aux.y - transform.position.y;
                float testZ = aux.z - transform.position.z;

                if ((testX + testY / 2) < (Mathf.Epsilon * 10))
                    path.Remove(path[0]);
            }
        });

        result.SetTransition(() =>
        {
            if (path.Count == 0)
            {
                OnFlag?.Invoke(Flags.OnReadyToMine);
            }
        });
        return result;
    }
}

