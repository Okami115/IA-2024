using System;
using UnityEngine;
using BehaivioursActions;

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaivioursAction GetOnEnterBehaviours(params object[] parameters);
    public abstract BehaivioursAction GetOnExitBehaviours(params object[] parameters);
    public abstract BehaivioursAction GetTickBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
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
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {

            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        });

        result.AddMultiThreadsBehaviours(0, () =>
        {

            Debug.Log("Chese is Active!");
        });
        result.SetTransition(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }
            else if (Vector3.Distance(TargetTransform.position, TargetTransform.position) > lostDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetLost);
            }
        });

        return result;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTargete;

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
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float cheseDistance = Convert.ToSingle(parameters[5]);

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(1, () =>
        {
            if (actualTargete == null)
            {
                actualTargete = wayPoint1;
            }

            if (Vector3.Distance(ownerTransform.position, actualTargete.position) < 0.2f)
            {
                if (actualTargete == wayPoint1)
                {
                    actualTargete = wayPoint2;
                }
                else if (actualTargete == wayPoint2)
                {
                    actualTargete = wayPoint1;
                }
            }

            Debug.Log("Patroling!");

            ownerTransform.position += (actualTargete.position - ownerTransform.position).normalized * speed * Time.deltaTime;

        });

        result.SetTransition(() =>
        {
            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < cheseDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetNear);
            }

        });
        return result;
    }
}

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

