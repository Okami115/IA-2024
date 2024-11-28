using BehaivioursActions;
using NeuralNet.Network;
using System;
using System.Numerics;

namespace herbivorous
{
    public class HerbivorousEatState : State
    {
        Vector2 position;
        private float previousDistance;
        private NeuralNetwork brain;
        private float positiveHalf;
        private float negativeHalf;

        public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
        {
            brain = parameters[0] as NeuralNetwork;
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
            bool hasEatenEnoughFood = (bool)parameters[3];
            int counterEating = (int)parameters[4];
            int maxEating = (int)parameters[5];
            var onHasEatenEnoughFood = parameters[6] as Action<bool>;
            var onEaten = parameters[7] as Action<int>;
            Plant plant = parameters[8] as Plant;
            behaviour.AddMultiThreadsBehaviours(0, () =>
            {
                if (plant == null)
                {
                    return;
                }

                if (outputs[0] >= 0f)
                {
                    if (position == nearFoodPos && !hasEatenEnoughFood)
                    {
                        if (plant.CanBeEaten())
                        {
                            plant.Eat();
                            onEaten(++counterEating);
                            brain.FitnessReward += 20;
                            if (counterEating == maxEating)
                            {
                                brain.FitnessReward += 30;
                                onHasEatenEnoughFood.Invoke(true);
                            }
                        }
                    }
                    else if (hasEatenEnoughFood || position != nearFoodPos)
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }
                }
                else
                {
                    if (position == nearFoodPos && !hasEatenEnoughFood)
                    {
                        brain.FitnessMultiplier -= 0.05f;
                    }
                    else if (hasEatenEnoughFood)
                    {
                        brain.FitnessMultiplier += 0.10f;
                    }
                }
            });
            return behaviour;
        }
    }

}