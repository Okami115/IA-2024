using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class WaitingFoodState : State
{
    private Mine<Vector2Int> mine;
    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        Node<Vector2Int> currentNode = parameters[0] as Node<Vector2Int>;
        List<Mine<Vector2Int>> mines = parameters[1] as List<Mine<Vector2Int>>;

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
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        BehaivioursAction result = new BehaivioursAction();

        result.SetTransition(() =>
        {
            if(mine.currentFood > 0)
            {
                Debug.Log("*Le tiran un pan*");
                OnFlag?.Invoke(Flags.OnReadyToMine);
            }
        });

        return result;
    }
}

