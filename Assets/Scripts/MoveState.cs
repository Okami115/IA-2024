using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoveState : State
{
    private Pathfinder<Node<Vector3>, Vector3> pathfinder;
    private List<Node<Vector3>> path = new List<Node<Vector3>>();

    private GrapfView grapfView;

    private Inventory inventory;

    bool isReturnHome = false;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        BehaivioursAction result = new BehaivioursAction();

        Node<Vector3> currentNode = parameters[0] as Node<Vector3>;
        grapfView = parameters[1] as GrapfView;
        inventory = parameters[2] as Inventory;
        PolygonCutter polygonCutter = new PolygonCutter();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            pathfinder = new AStarPathfinder<Node<Vector3>, Vector3>();

            if (inventory.gold >= 15 || grapfView.mines.Count == 0 || grapfView.isAlert)
            {
                path = pathfinder.FindPath(currentNode, grapfView.urbanCenter, grapfView.grapf.nodes) as List<Node<Vector3>>;

                if(grapfView.isAlert)
                    isReturnHome = true;
            }
            else
            {

                for (int i = 0; i < grapfView.mines.Count; i++)
                {
                    if(polygonCutter.IsPointInPolygon(currentNode.GetCoordinate(), grapfView.mines[i].poligon))
                    {
                        path = pathfinder.FindPath(currentNode, grapfView.mines[i].currentNode, grapfView.grapf.nodes) as List<Node<Vector3>>;
                        isReturnHome = false;
                    }
                }
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
        int OffsetPublic = Convert.ToInt32(parameters[1]);
        float speed = Convert.ToSingle(parameters[2]);
        Traveler traveler = parameters[3] as Traveler;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {
            if (path.Count > 0)
            {
                Vector3 aux = new Vector3(OffsetPublic * path[0].GetCoordinate().x, OffsetPublic * path[0].GetCoordinate().y, OffsetPublic * path[0].GetCoordinate().z);

                transform.position += (aux - transform.position).normalized * speed * Time.deltaTime;

                float dist = Vector3.Distance(transform.position, aux);
                float minDist = 0.1f;
                
                //////
                if (dist < minDist)
                {
                    traveler.currentNode = path[0];
                    path.Remove(path[0]);
                }
            }
        });

        result.SetTransition(() =>
        {
            if(grapfView.isAlert && !isReturnHome)
            {
                OnFlag?.Invoke(Flags.IsAlert);
            }

            if (!grapfView.isAlert && isReturnHome)
            {
                OnFlag?.Invoke(Flags.IsAlert);
            }

            if (path.Count == 0)
            {
                if (inventory.gold < 15 && !grapfView.isAlert)
                {
                    OnFlag?.Invoke(Flags.OnReadyToMine);
                }
                else
                {
                    OnFlag?.Invoke(Flags.IsInHome);
                }
            }

        });
        return result;
    }
}

