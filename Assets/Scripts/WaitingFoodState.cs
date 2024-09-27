using BehaivioursActions;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaitingFoodState : State
{
    private Mine<Vector3> mine;
    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector3> currentNode = parameters[0] as Node<Vector3>;
        List<Mine<Vector3>> mines = parameters[1] as List<Mine<Vector3>>;

        BehaivioursAction result = new BehaivioursAction();
        result.AddMainThreadBehaviours(0, () =>
        {
            for (int i = 0; i < mines.Count; i++)
            {
                if (mines[i].currentNode == currentNode)
                {
                    mine = mines[i];
                    break;
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
        GrapfView grapfView = parameters[0] as GrapfView;

        BehaivioursAction result = new BehaivioursAction();

        result.SetTransition(() =>
        {
            if(grapfView.isAlert)
            {
                OnFlag?.Invoke(Flags.IsAlert);
            }

            if (mine.currentFood > 0)
            {
                Debug.Log("*Le tiran un pan*");
                OnFlag?.Invoke(Flags.OnReadyToMine);
            }
        });

        return result;
    }
}

