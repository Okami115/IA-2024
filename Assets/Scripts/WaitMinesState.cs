using BehaivioursActions;
using UnityEngine;

public sealed class WaitMinesState : State
{
    private GrapfView grapfView;
    private Traveler traveler;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        grapfView = parameters[0] as GrapfView;
        traveler = parameters[1] as Traveler;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
            grapfView.currentGold += traveler.inventory.gold;
            traveler.inventory.gold = 0;
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


        result.SetTransition(() =>
        {
            if (!grapfView.isAlert)
            {
                OnFlag?.Invoke(Flags.OnReadyToBack);
            }
        });
        return result;
    }
}
