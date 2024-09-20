using BehaivioursActions;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class MiningState : State
{
    private Mine<Vector2Int> mine;
    private Inventory inventory;

    float time = 0f;
    int counter = 0;
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
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        float minningSpeed = Convert.ToSingle(parameters[0]);
        float deltaTime = Convert.ToSingle(parameters[1]);
        List<Mine<Vector2Int>> mines = parameters[2] as List<Mine<Vector2Int>>;

        BehaivioursAction result = new BehaivioursAction();

        result.AddMultiThreadsBehaviours(0, () => 
        {
            if(mine.currentGold > 0)
            {
                time += deltaTime;

                if (time > minningSpeed)
                {
                    inventory.gold++;
                    mine.currentGold--;
                    counter++;
                    time = 0;
                }

                if(counter >= 3)
                {
                    mine.currentFood--;
                    counter = 0;
                }
            }                
        });

        result.SetTransition(() =>
        {
            if(mine.currentFood == 0)
            {
                Debug.Log("No more food");
                OnFlag?.Invoke(Flags.OnReadyToEat);
            }

            if (mine.currentGold <= 0)
            {
                Debug.Log("No more gold");
                OnFlag?.Invoke(Flags.OnReadyToBack);
                mines.Remove(mine);
            }

            if (inventory.gold >= 15)
            {
                Debug.Log("Full of Gold");
                OnFlag?.Invoke(Flags.OnReadyToBack);
            }

        });

        return result;
    }
}

