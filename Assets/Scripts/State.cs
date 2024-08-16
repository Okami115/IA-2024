using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public struct behaivioursAction
{
    private Dictionary<int, List<Action>> mainThreadsBehaiviours;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multiThreadsBehaiviours;
    private Action transitionBehaiviours;

    public void AddMainThreadBehaviours(int executionOrder, Action behaviour)
    {
        if (mainThreadsBehaiviours == null)
            mainThreadsBehaiviours = new Dictionary<int, List<Action>>();

        if (!mainThreadsBehaiviours.ContainsKey(executionOrder))
            mainThreadsBehaiviours.Add(executionOrder, new List<Action>());

        mainThreadsBehaiviours[executionOrder].Add(behaviour);
    }

    public void AddMultiThreadsBehaviours(int executionOrder, Action behaviour)
    {
        if (multiThreadsBehaiviours == null)
            multiThreadsBehaiviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();

        if (!multiThreadsBehaiviours.ContainsKey(executionOrder))
            multiThreadsBehaiviours.TryAdd(executionOrder, new ConcurrentBag<Action>());

        multiThreadsBehaiviours[executionOrder].Add(behaviour);
    }

    public void SetTransition(Action behaviours)
    {
        transitionBehaiviours = behaviours;
    }

    public Dictionary<int, List<Action>> MainThreadBahaviours => mainThreadsBehaiviours;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultiThreadBehaviours => multiThreadsBehaiviours;
    public Action TransitionBehaviours => transitionBehaiviours;
}

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract behaivioursAction GetOnEnterBehaviours(params object[] parameters);
    public abstract behaivioursAction GetOnExitBehaviours(params object[] parameters);
    public abstract behaivioursAction GetTickBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override behaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        behaivioursAction result = new behaivioursAction();

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

    public override behaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetTickBehaviours(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float cheseDistance = Convert.ToSingle(parameters[5]);

        behaivioursAction result = new behaivioursAction();

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
    public override behaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override behaivioursAction GetTickBehaviours(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);

        behaivioursAction result = new behaivioursAction();
        result.AddMultiThreadsBehaviours(0, () =>
        {
            Debug.Log("BOOM!");
        });

        return result;
    }
}

