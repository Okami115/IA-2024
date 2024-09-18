using BehaivioursActions;
using System;
using UnityEngine;

public sealed class MiningState : State
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
        float? gold = parameters[0] as float?;
        float speedToMine = Convert.ToSingle(parameters[1]);
        float goldInTheMine = Convert.ToSingle(parameters[2]);

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {
            gold += speedToMine * Time.deltaTime;
            goldInTheMine -= speedToMine * Time.deltaTime;

            Debug.Log($"Current Gold :: {gold}, Restant Gold :: {goldInTheMine}");
        });

        result.SetTransition(() =>
        {
            if(goldInTheMine <= 0)
            {
                Debug.Log("No Gold");
            }
            else if(gold >= 15)
            {
                Debug.Log("Full Gold");
            }
        });
        return result;
    }
}

