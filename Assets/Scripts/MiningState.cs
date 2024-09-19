using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MiningState : State
{
    private Mine<Vector2Int> mine;
    private Inventory inventory;
    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector2Int> currentNode = parameters[0] as Node<Vector2Int>;
        inventory = parameters[1] as Inventory;
        List<Mine<Vector2Int>> mines = parameters[2] as List<Mine<Vector2Int>>;

        BehaivioursAction result = new BehaivioursAction();
        result.AddMainThreadBehaviours(0, () =>
        { 
            for (int i = 0; i < mines.Count; i++)
            {
                if (mines[i].currentNode == currentNode)
                {
                    mine = mines[i];
                    break;
                }
            }

        });

        return result;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        Traveler traveler = parameters[0] as Traveler;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () =>
        {
            traveler.destinationNode = traveler.grapfView.urbanCenter;
        });

        return result;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        float minningSpeed = Convert.ToSingle(parameters[0]);

        BehaivioursAction result = new BehaivioursAction();

        result.AddMainThreadBehaviours(0, () => 
        {
            if(mine.currentGold > 0)
            {
                inventory.gold += Time.deltaTime * minningSpeed;
                mine.currentGold -= Time.deltaTime * minningSpeed;
            }                
        });

        result.SetTransition(() =>
        {
            if(inventory.gold >= 15)
            {
                Debug.Log("Full of Gold");
                OnFlag?.Invoke(Flags.OnReadyToBack);
            }
        });
        return result;
    }
}

