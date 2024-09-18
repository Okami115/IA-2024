using BehaivioursActions;
using System;
using UnityEngine;

public sealed class ExplodeState : State
{
    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);

        BehaivioursAction result = new BehaivioursAction();
        result.AddMultiThreadsBehaviours(0, () =>
        {
            Debug.Log("BOOM!");
        });

        return result;
    }
}

