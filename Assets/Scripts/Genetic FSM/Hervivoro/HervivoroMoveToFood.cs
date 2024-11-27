using System;
using System.Collections.Generic;
using System.Numerics;
using BehaivioursActions;

public class HervivoroMoveToFood : State
{
    List<Vector2> nearEnemyPositions = new List<Vector2>();
    private float previousDistance;
    private NeuralNetwork brain;
    private Vector2 position;
    private float positiveHalf;
    private float negativeHalf;

    public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
    {
        brain = parameters[0] as NeuralNetwork;
        return default;
    }

    public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
    {
        brain.ApplyFitness();
        return default;
    }

    public override BehaivioursAction GetTickBehaviours(params object[] parameters)
    {
        BehaivioursAction behaviour = new BehaivioursAction();

        float[] outputs = parameters[0] as float[];
        position = (Vector2)parameters[1];
        Vector2 nearFoodPos = (Vector2)parameters[2];
        var onMove = parameters[3] as Action<Vector2>;
        Plant plant = parameters[4] as Plant;
        int gridSizeX = (int)parameters[5];
        int gridSizeY = (int)parameters[6];
        behaviour.AddMultiThreadsBehaviours(0, () =>
        {
            //Outputs:
            //
            //0 cuanto se mueve
            //1 a 3 es direction

            positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
            negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);

            int movementPerTurn = 0;

            if (outputs[0] > positiveHalf)
            {
                movementPerTurn = 3;
            }
            else if (outputs[0] < positiveHalf && outputs[0] > 0)
            {
                movementPerTurn = 2;
            }
            else if (outputs[0] < 0 && outputs[0] < negativeHalf)
            {
                movementPerTurn = 1;
            }
            else if (outputs[0] < negativeHalf)
            {
                movementPerTurn = 0;
            }

            Vector2[] direction = new Vector2[movementPerTurn];
            for (int i = 0; i < movementPerTurn; i++)
            {
                direction[i] = GetDir(outputs[i + 1]);
            }

            if (movementPerTurn > 0)
            {
                foreach (Vector2 dir in direction)
                {
                    onMove.Invoke(dir);
                    position += dir;

                    if (position.X > gridSizeX)
                    {
                        position.X = gridSizeX;
                    }
                    else if (position.X < 0)
                    {
                        position.X = 0;
                    }

                    if (position.Y > gridSizeY)
                    {
                        position.Y = gridSizeY;
                    }
                    else if (position.Y < 0)
                    {
                        position.Y = 0;
                    }
                }

                List<Vector2> newPositions = new List<Vector2>();
                newPositions.Add(nearFoodPos);
                float distanceFromFood = GetDistanceFrom(newPositions);
                if (distanceFromFood >= previousDistance)
                {
                    brain.FitnessReward += 20;
                    brain.FitnessMultiplier += 0.05f;
                }
                else
                {
                    brain.FitnessMultiplier -= 0.05f;
                }

                previousDistance = distanceFromFood;
            }
        });
        return behaviour;
    }

    protected Vector2 GetDir(float x)
    {
        Vector2 dir = new Vector2();
        if (x > positiveHalf)
        {
            dir = new Vector2(1, 0);
        }
        else if (x < positiveHalf && x > 0)
        {
            dir = new Vector2(-1, 0);
        }
        else if (x < 0 && x < negativeHalf)
        {
            dir = new Vector2(0, 1);
        }
        else if (x < negativeHalf)
        {
            dir = new Vector2(0, -1);
        }

        return dir;
    }

    protected float GetDistanceFrom(List<Vector2> enemies)
    {
        float distance = float.MaxValue;
        foreach (var enemy in enemies)
        {
            float newDistance = Vector2.Distance(position, enemy);
            if (distance > newDistance)
            {
                distance = newDistance;
            }
        }

        return distance;
    }
}