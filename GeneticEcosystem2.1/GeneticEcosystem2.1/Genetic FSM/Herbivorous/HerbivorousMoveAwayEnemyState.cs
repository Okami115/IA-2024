using BehaivioursActions;
using NeuralNet.Components;
using NeuralNet.Network;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace herbivorous
{
    public class HerbivorousMoveAwayEnemyState : State
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
            positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
            negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);
            return default;
        }

        public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaivioursAction GetTickBehaviours(params object[] parameters)
        {
            BehaivioursAction behaviour = new BehaivioursAction();

            float[] outputs = parameters[0] as float[];
            position = (Vector2)parameters[1];
            nearEnemyPositions = parameters[2] as List<Vector2>;
            Action<Vector2> onMove = parameters[3] as Action<Vector2>;
            int gridSizeX = (int)parameters[4];
            int gridSizeY = (int)parameters[5];
            behaviour.AddMultiThreadsBehaviours(0, () =>
            {
                int movementPerTurn = 0;

                positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
                negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);

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
                for (int i = 0; i < direction.Length; i++)
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
                            position.X = 0;
                        }
                        else if (position.X < 0)
                        {
                            position.X = gridSizeX;
                        }

                        if (position.Y > gridSizeY)
                        {
                            position.Y = 0;
                        }
                        else if (position.Y < 0)
                        {
                            position.Y = gridSizeY;
                        }
                    }

                    float distanceFromEnemies = GetDistanceFrom(nearEnemyPositions);
                    if (distanceFromEnemies <= previousDistance)
                    {
                        brain.FitnessReward += 20;
                        brain.FitnessMultiplier += 0.05f;
                    }
                    else
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }

                    previousDistance = distanceFromEnemies;
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

}