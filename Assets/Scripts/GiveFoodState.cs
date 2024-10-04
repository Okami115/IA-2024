using BehaivioursActions;
using UnityEngine;

public sealed class GiveFoodState : State
{
    private GrapfView grapfView;

    private Mine<Vector3> mine;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector3> currentNode = parameters[0] as Node<Vector3>;
        grapfView = parameters[1] as GrapfView;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            for (int i = 0; i < grapfView.mines.Count; i++)
            {
                if (grapfView.mines[i].currentNode == currentNode)
                {
                    mine = grapfView.mines[i];
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
        Caravana caravana = parameters[0] as Caravana;
        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            mine.currentFood = caravana.food;
            caravana.food = 0;
        });

        result.SetTransition(() =>
        {
            caravana.targetNode = grapfView.urbanCenter;
            OnFlag?.Invoke(Flags.OnReadyToTravel);
        });
        return result;
    }
}