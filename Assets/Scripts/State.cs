using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Action<int> OnFlag;
    public abstract List<Action> GetOnEnterBehaviours(params object[] parameters);
    public abstract List<Action> GetOnExitBehaviours(params object[] parameters);
    public abstract List<Action> GetTickBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);   

        List<Action> result = new List<Action>();

        result.Add(() => {

            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        });

        result.Add(() => {

            Debug.Log("Esto es raro");
        });
        result.Add(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetReach);
            }
            else if(Vector3.Distance(TargetTransform.position, TargetTransform.position) > lostDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetLost);
            }
        });

        return result;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTargete;

    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[2] as Transform;
        float speed = Convert.ToSingle(parameters[3]);
        float cheseDistance = Convert.ToSingle(parameters[4]);

        List<Action> result = new List<Action>();

        result.Add(() => {

            if(actualTargete == null)
            {
                actualTargete = wayPoint1;
            }

            if(Vector3.Distance(ownerTransform.position, actualTargete.position) < 0.2f)
            {
                if(actualTargete == wayPoint1)
                {
                    actualTargete = wayPoint2;
                }
                else if(actualTargete == wayPoint2)
                {
                    actualTargete = wayPoint1;
                }
            }

            ownerTransform.position += (actualTargete.position - ownerTransform.position).normalized * speed * Time.deltaTime;
        });

        result.Add(() =>
        {
            if(Vector3.Distance(ownerTransform.position, chaseTarget.position) < cheseDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetNear);
            }

        });
        return result;
    }
}

public sealed class ExplodeState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);

        List<Action> result = new List<Action>();
        result.Add(() => {

            Debug.Log("BOOM!");
        });

        return result;
    }
}

