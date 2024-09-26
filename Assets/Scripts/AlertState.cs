using BehaivioursActions;

public sealed class AlertState : State
{

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {

        BehaivioursAction result = new BehaivioursAction();
        result.AddMainThreadBehaviours(0, () =>
        {

        });

        return result;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        BehaivioursAction result = new BehaivioursAction();
        result.AddMainThreadBehaviours(0, () =>
        {

        });

        return result;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () =>
        {
        });

        result.SetTransition(() =>
        {

        });

        return result;
    }
}

