using System;
using System.Collections.Generic;
using System.Numerics;
using BehaivioursActions;

public sealed class ScavengerMoveState : State
{
    private float MinEatRadius;
    private int counter;
    private Vector2 dir;
    private Vector2 position;
    private float speed;
    private float radius;
    private NeuralNetwork flockingBrain;
    private NeuralNetwork brain;
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
        Hervivoro herbivore = parameters[5] as Hervivoro;
        var onMove = parameters[6] as Action<Vector2>;
        counter = (int)parameters[7];
        var onEat = parameters[8] as Action<int>;
        dir = (Vector2)parameters[9];
        float rotation = (float)(parameters[10]);
        speed = (float)(parameters[11]);
        radius = (float)(parameters[12]);
        List<Carroniero> nearScavengers = parameters[13] as List<Carroniero>;
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
            Vector2 flokingInfluence =
                dir * (flockingBrain.outputs[0] + flockingBrain.outputs[1] + flockingBrain.outputs[2]);

            Vector2 finalDirection = dir + flokingInfluence;

            finalDirection = Vector2.Normalize(finalDirection);
            onMove.Invoke(finalDirection);
            position += finalDirection * speed * deltaTime;
        });

        //fitness
        behaviour.AddMultiThreadsBehaviours(2, () =>
        {
            //fitness Floking
            foreach (Carroniero scavenger in nearScavengers)
            {
                //Alignment
                float diff = MathF.Abs(rotation - scavenger.rotation);

                if (diff > 180)
                    diff = 360 - diff;

                if (diff > 90)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }

                //Cohesion
                if (Vector2.Distance(position, scavenger.position) > radius * 6)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }

                //Separation
                if (Vector2.Distance(position, scavenger.position) < radius * 2)
                {
                    flockingBrain.FitnessMultiplier -= 0.05f;
                }
                else
                {
                    flockingBrain.FitnessReward += 1;
                }
            }
        });

        return behaviour;
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