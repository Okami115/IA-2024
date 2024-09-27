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
            foreach (Mine<Vector3> mine in grapfView.mines)
            {
                if (mine.currentFood <= 0 && mine.miners.Count > 0)
                {
                    onTargeSet = true;
                    break;
                }
            }
        });

        result.SetTransition(() =>
        {
            if (onTargeSet && !grapfView.isAlert)
            {
                OnFlag?.Invoke(Flags.OnReadyToTravel);
            }
        });
        return result;
    }
}
