using BehaivioursActions;
using UnityEngine;

public sealed class WaitOrdersState : State
{
    private GrapfView grapfView;
    private Caravana caravana;

    private bool onTargeSet;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        grapfView = parameters[0] as GrapfView;
        caravana = parameters[1] as Caravana;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            caravana.food = grapfView.foodForCaravana;
            onTargeSet = false;
        });

        return result;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            foreach (Mine<Vector2Int> mine in grapfView.mines)
            {
                if (mine.currentFood <= 0 && mine.miners.Count > 0)
                {
                    caravana.targetNode = mine.currentNode;
                    onTargeSet = true;
                }
            }
        });

        result.SetTransition(() =>
        {
            if (onTargeSet)
            {
                OnFlag?.Invoke(Flags.OnReadyToTravel);
            }
        });
        return result;
    }
}
