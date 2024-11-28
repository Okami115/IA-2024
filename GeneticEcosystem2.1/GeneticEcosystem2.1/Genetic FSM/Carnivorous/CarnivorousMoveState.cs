using BehaivioursActions;
using herbivorous;
using NeuralNet.Components;
using NeuralNet.Network;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace carnivorous
{
    public class CarnivorousMoveState : State
    {
        private int movesPerTurn = 2;
        private float previousDistance;
        private NeuralNetwork brain;
        private float positiveHalf;
        private float negativeHalf;
        private Vector2 position;

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
            Vector2 nearFoodPos = (Vector2)parameters[2];
            Action<Vector2> onMove = parameters[3] as Action<Vector2>;
            Herbivorous herbivore = parameters[4] as Herbivorous;
            int gridX = (int)parameters[5];
            int gridY = (int)parameters[6];
            behaviour.AddMultiThreadsBehaviours(0, () =>
            {
                if (position == nearFoodPos)
                {
                    if (herbivore.lives != 0)
                    {
                        herbivore.ReceiveDamage();
                        brain.FitnessReward += 50;
                        brain.FitnessMultiplier += 0.05f;
                    }
                }

                Vector2[] direction = new Vector2[movesPerTurn];
                for (int i = 0; i < direction.Length; i++)
                {
                    direction[i] = GetDir(outputs[i]);
                }

                foreach (Vector2 dir in direction)
                {
                    onMove?.Invoke(dir);
                    position += dir;
                    if (position.X > gridX)
                    {
                        position.X = 0;
                    }
                    else if (position.X < 0)
                    {
                        position.X = gridX;
                    }

                    if (position.Y > gridY)
                    {
                        position.Y = 0;
                    }
                    else if (position.Y < 0)
                    {
                        position.Y = gridY;
                    }
                }

                List<Vector2> newPositions = new List<Vector2> { nearFoodPos };
                float distanceFromFood = GetDistanceFrom(newPositions);
                if (distanceFromFood <= previousDistance)
                {
                    brain.FitnessReward += 20;
                    brain.FitnessMultiplier += 0.05f;
                }
                else
                {
                    brain.FitnessMultiplier -= 0.05f;
                }

                previousDistance = distanceFromFood;
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