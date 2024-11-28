using manager;
using NeuralNet.Network;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace herbivorous
{
    public class Herbivorous
    {
        List<Vector2> nearEnemy = new List<Vector2>();
        List<Vector2> nearFood = new List<Vector2>();
        public Vector2 position;
        public int lives = 3;
        private int livesUntilCountdownDissapears = 30;
        private int maxFood = 5;
        int currentFood = 0;
        public bool hasEatenFood = false;
        private int maxMovementPerTurn = 3;
        public NeuralNetwork moveBrain;
        public NeuralNetwork runBrain;
        public NeuralNetwork eatBrain;
        public NeuralNetwork mainBrain;
        public FSM<HeribovoreStates, HerbivoreFlags> fsm;
        private EcosystemManager populationManager;
        private Vector2 nearFoodPosition;
        private Plant nearestFood;
        private List<Vector2> nearEnemiesPositions;

        public Herbivorous(EcosystemManager populationManager, NeuralNetwork main, NeuralNetwork moveBrain, NeuralNetwork eatBrain, NeuralNetwork runBrain)
        {
            fsm = new FSM<HeribovoreStates, HerbivoreFlags>();
            this.populationManager = populationManager;
            nearEnemiesPositions = new List<Vector2>();
            this.moveBrain = moveBrain;
            this.eatBrain = eatBrain;
            this.runBrain = runBrain;
            this.mainBrain = main;
            Action<Vector2> onMove;
            Action<bool> onEatenFood;
            Action<int> onEat;
            onMove = MoveTo;
            fsm.AddBehaviour<HerbivorousMoveToFood>(HeribovoreStates.Move,
                onEnterParameters: () => { return new object[] { moveBrain }; },
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        moveBrain.outputs, position, GetNearestFoodPosition(), onMove, GetNearestFood(),
                        populationManager.gridSizeX, populationManager.gridSizeY
                    };
                });
            fsm.AddBehaviour<HerbivorousEatState>(HeribovoreStates.Eat,
                onEnterParameters: () => { return new object[] { eatBrain }; },
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        eatBrain.outputs, position, GetNearestFood().position, hasEatenFood, currentFood, maxFood,
                        onEatenFood = b => { hasEatenFood = b; }, onEat = a => currentFood = a, GetNearestFood(),
                    };
                });
            fsm.AddBehaviour<HerbivorousMoveAwayEnemyState>(HeribovoreStates.Escape,
                onEnterParameters: () => new object[] { runBrain },
                onTickParameters: () =>
                {
                    return new object[]
                    {
                        runBrain.outputs, position, GetNearEnemiesPositions(), onMove = MoveTo,
                        populationManager.gridSizeX, populationManager.gridSizeY
                    };
                }
            );
            fsm.AddBehaviour<HerbivorousDeadState>(HeribovoreStates.Dead,
                onTickParameters: () => { return new object[] { lives }; });
            fsm.AddBehaviour<HerbivorousCorpseState>(HeribovoreStates.Corpse);

            fsm.SetTrasnsition(HeribovoreStates.Move, HerbivoreFlags.ToEat, HeribovoreStates.Eat);
            fsm.SetTrasnsition(HeribovoreStates.Move, HerbivoreFlags.ToEscape, HeribovoreStates.Escape);
            fsm.SetTrasnsition(HeribovoreStates.Escape, HerbivoreFlags.ToEat, HeribovoreStates.Eat);
            fsm.SetTrasnsition(HeribovoreStates.Escape, HerbivoreFlags.ToMove, HeribovoreStates.Move);
            fsm.SetTrasnsition(HeribovoreStates.Eat, HerbivoreFlags.ToMove, HeribovoreStates.Move);
            fsm.SetTrasnsition(HeribovoreStates.Eat, HerbivoreFlags.ToEscape, HeribovoreStates.Escape);
            fsm.SetTrasnsition(HeribovoreStates.Dead, HerbivoreFlags.ToCorpse, HeribovoreStates.Corpse);
            fsm.ForceState(HeribovoreStates.Move);
        }

        public void DecideState(float[] outputs)
        {
            int maxIndex = 2;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > outputs[maxIndex])
                {
                    maxIndex = i;
                }
            }

            switch (maxIndex)
            {
                case 0:
                    fsm.Transition(HeribovoreStates.Escape);

                    break;
                case 1:
                    fsm.Transition(HeribovoreStates.Move);

                    break;
                case 2:
                    fsm.Transition(HeribovoreStates.Eat);

                    break;
                default:
                    break;
            }
        }

        public void PreUpdate(float deltaTime)
        {
            nearFoodPosition = GetNearestFoodPosition();
            nearestFood = GetNearestFood();
            nearEnemiesPositions = GetNearEnemiesPositions();

            mainBrain.inputs = new[]
            {
                position.X, position.Y, nearFoodPosition.X, nearFoodPosition.Y, hasEatenFood ? 1 : -1,
                nearEnemiesPositions[0].X, nearEnemiesPositions[0].Y, nearEnemiesPositions[1].X,
                nearEnemiesPositions[1].Y, nearEnemiesPositions[2].X,
                nearEnemiesPositions[2].Y
            };
            moveBrain.inputs = new[] { position.X, position.Y, nearFoodPosition.X, nearFoodPosition.Y };
            eatBrain.inputs = new[]
                { position.X, position.Y, nearFoodPosition.X, nearFoodPosition.Y, hasEatenFood ? 1 : -1 };

            runBrain.inputs = new[]
            {
                position.X, position.Y, nearEnemiesPositions[0].X, nearEnemiesPositions[0].Y, nearEnemiesPositions[1].X,
                nearEnemiesPositions[1].Y, nearEnemiesPositions[2].X,
                nearEnemiesPositions[2].Y
            };
        }

        public void Update(float deltaTime)
        {
            DecideState(mainBrain.outputs);
            fsm.Tick();
        }

        public void MoveTo(Vector2 dir)
        {
            position += dir;
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

        public bool IsCorpse()
        {
            return fsm.currentState == (int)HeribovoreStates.Corpse;
        }

        public void GiveFitnessToMain()
        {
            runBrain.ApplyFitness();
            moveBrain.ApplyFitness();
            eatBrain.ApplyFitness();

            mainBrain.FitnessMultiplier = 1.0f;
            mainBrain.FitnessReward = 0f;
            mainBrain.FitnessReward += eatBrain.FitnessReward + moveBrain.FitnessReward + runBrain.FitnessReward;
            mainBrain.FitnessMultiplier += eatBrain.FitnessMultiplier + moveBrain.FitnessMultiplier +
                                           runBrain.FitnessMultiplier;

            mainBrain.ApplyFitness();
        }

        public List<Vector2> GetNearEnemiesPositions()
        {
            return populationManager.GetNearCarnivores(position);
        }

        public void ReceiveDamage()
        {
            lives--;
            if (lives <= 0)
            {
                fsm.ForceState(HeribovoreStates.Dead);
            }
        }

        public void Reset(Vector2 position)
        {
            mainBrain.FitnessMultiplier = 1.0f;
            mainBrain.FitnessReward = 0f;
            eatBrain.FitnessMultiplier = 1.0f;
            eatBrain.FitnessReward = 0f;
            moveBrain.FitnessMultiplier = 1.0f;
            moveBrain.FitnessReward = 0f;
            runBrain.FitnessMultiplier = 1.0f;
            runBrain.FitnessReward = 0f;


            lives = 3;
            currentFood = 0;
            hasEatenFood = false;
            this.position = position;
            fsm.ForceState(HeribovoreStates.Move);
        }

        public bool CanBeEaten()
        {
            if (fsm.currentState == (int)HeribovoreStates.Dead)
            {
                fsm.ForceState(HeribovoreStates.Corpse);
                return true;
            }

            return false;
        }

        public Plant GetNearestFood()
        {
            return populationManager.GetNearPlant(position);
        }

        public Vector2 GetNearestFoodPosition()
        {
            return GetNearestFood().position;
        }
    }

    public enum HerbivoreFlags
    {
        ToEat,
        ToEscape,
        ToMove,
        ToCorpse
    }

    public enum HeribovoreStates
    {
        Move,
        Eat,
        Escape,
        Dead,
        Corpse
    }
}

public class Plant
{
    private int lives = 5;
    public bool isAvailable = true;
    public Vector2 position;

    public void Eat()
    {
        if (isAvailable)
        {
            lives--;

            if (lives <= 0)
            {
                isAvailable = false;
            }
        }
    }

    public bool CanBeEaten()
    {
        return lives > 0;
    }

    public void Reset(Vector2 position)
    {
        this.position = position;
        int lives = 5;
        isAvailable = true;
    }
}