using manager;
using herbivorous;
using System;
using System.Collections.Generic;
using System.Numerics;
using NeuralNet.Network;

namespace scavenger
{
    public class Scavenger
    {
        public NeuralNetwork flockingBrain;
        public NeuralNetwork mainBrain;
        public Vector2 position;
        float minEatRadius;
        public Vector2 dir = new Vector2(1, 1);
        public bool hasEaten = false;
        public int counterEating = 0;
        public float rotation = 0;
        public float speed = 5;
        public float radius = 2;
        private FSM<ScavengerStates, ScavengerFlags> fsm;
        protected EcosystemManager populationManager;
        float deltaTime = 0;

        public void Reset(Vector2 position)
        {
            hasEaten = false;
            this.position = position;
            counterEating = 0;
            fsm.ForceState(ScavengerStates.Move);
        }

        public Scavenger(EcosystemManager populationManager, NeuralNetwork main, NeuralNetwork flockBrain)
        {
            fsm = new FSM<ScavengerStates, ScavengerFlags>();
            this.populationManager = populationManager;
            flockingBrain = flockBrain;
            minEatRadius = 4f;
            mainBrain = main;

            Action<Vector2> setDir;
            Action<int> setEatingCounter;
            fsm.AddBehaviour<ScavengerMoveState>(ScavengerStates.Move,
                onEnterParameters: () => { return new object[] { mainBrain, position, minEatRadius, flockingBrain }; },
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        mainBrain.outputs, position, GetNearFoodPos(), minEatRadius, hasEaten, GetNearHerbivore(),
                        setDir = MoveTo, counterEating, setEatingCounter = b => counterEating = b, dir, rotation, speed,
                        radius, GetNearScavs(), deltaTime
                    };
                });

            fsm.ForceState(ScavengerStates.Move);
        }

        public void PreUpdate(float deltaTime)
        {
            this.deltaTime = deltaTime;
            var nearFoodPos = GetNearFoodPos();
            mainBrain.inputs = new[] { position.X, position.Y, minEatRadius, nearFoodPos.X, nearFoodPos.Y };

            var ner = GetNearScavs();
            flockingBrain.inputs = new[]
            {
                position.X, position.Y, ner[0].position.X, ner[0].position.Y, ner[1].position.X, ner[1].position.Y,
                ner[0].rotation, ner[1].rotation
            };
        }

        public void Update(float deltaTime)
        {
            fsm.Tick();
            Move(deltaTime);
        }

        private void Move(float deltaTime)
        {
            position += dir * speed * deltaTime;
            if (position.X > populationManager.gridSizeX)
            {
                position.X = 0;
            }
            else if (position.X < 0)
            {
                position.X = populationManager.gridSizeX;
            }

            if (position.Y > populationManager.gridSizeY)
            {
                position.Y = 0;
            }
            else if (position.Y < 0)
            {
                position.Y = populationManager.gridSizeY;
            }
        }

        public Vector2 GetNearFoodPos()
        {
            return GetNearHerbivore().position;
        }

        public Herbivorous GetNearHerbivore()
        {
            return populationManager.GetNearHerbivore(position);
        }

        public List<Scavenger> GetNearScavs()
        {
            return populationManager.GetNearScavs(this);
        }

        public void MoveTo(Vector2 dir)
        {
            this.dir = dir;
        }

        public void GiveFitnessToMain()
        {
            flockingBrain.ApplyFitness();

            mainBrain.FitnessMultiplier = 1.0f;
            mainBrain.FitnessReward = 0f;
            mainBrain.FitnessReward += flockingBrain.FitnessReward + (hasEaten ? flockingBrain.FitnessReward : 0);
            mainBrain.FitnessMultiplier += flockingBrain.FitnessMultiplier + (hasEaten ? 1 : 0);

            mainBrain.ApplyFitness();
        }
    }

    public enum ScavengerStates
    {
        Move
    }

    public enum ScavengerFlags
    {
        ToMove
    }
}