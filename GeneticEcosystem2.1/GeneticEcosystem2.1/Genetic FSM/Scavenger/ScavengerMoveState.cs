using System;
using System.Collections.Generic;
using System.Numerics;
using BehaivioursActions;
using herbivorous;
using NeuralNet.Components;
using NeuralNet.Network;

namespace scavenger
{
    public sealed class ScavengerMoveState : State
    {
        private Vector2 position;
        private NeuralNetwork brain;
        private float MinEatRadius;
        private int counter;
        private Vector2 dir;
        private float speed;
        private float radius;
        private NeuralNetwork flockingBrain;
        Vector2 Aligment;
        Vector2 Cohesion;
        Vector2 Separation;
        private float positiveHalf;
        private float negativeHalf;

        public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
        {
            brain = parameters[0] as NeuralNetwork;
            position = (Vector2)(parameters[1]);
            MinEatRadius = (float)(parameters[2]);
            positiveHalf = Neuron.Sigmoid(0.5f, brain.p);
            negativeHalf = Neuron.Sigmoid(-0.5f, brain.p);
            flockingBrain = parameters[3] as NeuralNetwork;
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
            position = (Vector2)(parameters[1]);
            Vector2 nearFoodPos = (Vector2)parameters[2];
            MinEatRadius = (float)(parameters[3]);
            bool hasEatenFood = (bool)parameters[4];
            Herbivorous herbivore = parameters[5] as Herbivorous;
            var onMove = parameters[6] as Action<Vector2>;
            counter = (int)parameters[7];
            var onEat = parameters[8] as Action<int>;
            dir = (Vector2)parameters[9];
            float rotation = (float)(parameters[10]);
            speed = (float)(parameters[11]);
            radius = (float)(parameters[12]);
            List<Scavenger> nearScavengers = parameters[13] as List<Scavenger>;
            float deltaTime = (float)parameters[14];
            behaviour.AddMultiThreadsBehaviours(0, () =>
            {
                List<Vector2> newPositions = new List<Vector2> { nearFoodPos };
                float distanceFromFood = GetDistanceFrom(newPositions);

                if (distanceFromFood < MinEatRadius && !hasEatenFood)
                {
                    counter++;
                    onEat.Invoke(counter);
                    brain.FitnessReward += 1;

                    if (counter >= 20)
                    {
                        brain.FitnessReward += 20;
                        brain.FitnessMultiplier += 0.10f;
                        hasEatenFood = true;
                    }
                }
                else if (distanceFromFood > MinEatRadius)
                {
                    brain.FitnessMultiplier -= 0.05f;
                }

                float leftValue = outputs[0];
                float rightValue = outputs[1];

                float netRotationValue = leftValue - rightValue;
                float turnAngle = netRotationValue * MathF.PI / 180;

                var rotationMatrix = new Matrix3x2(
                    MathF.Cos(turnAngle), MathF.Sin(turnAngle),
                    -MathF.Sin(turnAngle), MathF.Cos(turnAngle),
                    0, 0
                );

                dir = Vector2.Transform(dir, rotationMatrix);
                dir = Vector2.Normalize(dir);
                rotation += netRotationValue;

                rotation = (rotation + 360) % 360;
            });

            behaviour.AddMultiThreadsBehaviours(1, () =>
            {
                Aligment = new Vector2(flockingBrain.outputs[0], flockingBrain.outputs[1]);
                Aligment = Vector2.Normalize(Aligment);
                Cohesion = new Vector2(flockingBrain.outputs[2], flockingBrain.outputs[3]);
                Cohesion = Vector2.Normalize(Cohesion);
                Separation = new Vector2(flockingBrain.outputs[4], flockingBrain.outputs[5]);
                Separation = Vector2.Normalize(Separation);
                float aligmentWeight = 2;
                float cohesionWeight = 2;
                float separationWeight = 2;
                Vector2 flokingInfluence = Aligment * aligmentWeight + Cohesion * cohesionWeight +
                                           Separation * separationWeight + dir;


                Vector2 finalDirection = Vector2.Normalize(flokingInfluence);
                onMove.Invoke(finalDirection);
                position += finalDirection * speed * deltaTime;


                if (AlignmentCalc(nearScavengers) != Aligment)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }

                //Cohesion
                if (CohesionCalc(nearScavengers) != Cohesion)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }

                //Separation
                if (SeparationCalc(nearScavengers) != Separation)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }
            });


            return behaviour;
        }

        public Vector2 AlignmentCalc(List<Scavenger> scavengers)
        {
            Vector2 avg = Vector2.Zero;
            foreach (Scavenger b in scavengers)
            {
                avg += b.dir * b.speed;
            }

            avg /= scavengers.Count;
            avg = Vector2.Normalize(avg);
            return avg;
        }

        public Vector2 CohesionCalc(List<Scavenger> scavengers)
        {
            Vector2 avg = Vector2.Zero;
            foreach (Scavenger b in scavengers)
            {
                avg += b.position;
            }

            avg /= scavengers.Count;
            avg = (avg - position);
            avg = Vector2.Normalize(avg);
            return avg;
        }

        public Vector2 SeparationCalc(List<Scavenger> scavengers)
        {
            Vector2 avg = Vector2.Zero;
            foreach (Scavenger b in scavengers)
            {
                avg += (b.position - position);
            }

            avg /= scavengers.Count;
            avg *= -1;
            avg = Vector2.Normalize(avg);
            return avg;
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