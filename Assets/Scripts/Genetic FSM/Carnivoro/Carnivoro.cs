using System;
using System.Numerics;

public class Carnivoro
{
    public NeuralNetwork moveBrain;
    public NeuralNetwork eatBrain;
    public NeuralNetwork mainBrain;
    int counterEating = 0;
    int maxEating = 3;
    public bool hasEatenEnoughFood = false;
    public Vector2 position;

    protected SporeManager populationManager;
    private FSM<CarnivoreStates, CarnivoreFlags> fsm;

    public Carnivoro(SporeManager populationManager, NeuralNetwork main, NeuralNetwork move, NeuralNetwork eat)
    {
        mainBrain = main;
        this.populationManager = populationManager;
        fsm = new FSM<CarnivoreStates, CarnivoreFlags>();
        moveBrain = move;
        eatBrain = eat;
        Action<bool> onHasEantenEnoughFood;
        Action<Vector2> onMove;
        Action<int> onEaten;
        fsm.AddBehaviour<CarnivoreEatState>(CarnivoreStates.Eat,
            onEnterParameters: () => { return new object[] { eatBrain }; }, onTickParameters: () =>
            {
                return new object[]
                {
                    eatBrain.outputs, position, GetNearFoodPos(),
                    hasEatenEnoughFood, counterEating, maxEating,
                    onHasEantenEnoughFood = b =>
                        hasEatenEnoughFood = b,
                    onEaten = i => counterEating = i,
                    GetNearHerbivore()
                };
            });
        fsm.AddBehaviour<CarnivoreMoveState>(CarnivoreStates.Move,
            onEnterParameters: () => { return new object[] { eatBrain }; }, onTickParameters: () =>
            {
                return new object[]
                {
                    moveBrain.outputs, position, GetNearFoodPos(),
                    onMove = MoveTo,
                    GetNearHerbivore()
                };
            });
        fsm.SetTrasnsition(CarnivoreStates.Eat, CarnivoreFlags.ToMove, CarnivoreStates.Move);
        fsm.SetTrasnsition(CarnivoreStates.Move, CarnivoreFlags.ToEat, CarnivoreStates.Eat);
        fsm.ForceState(CarnivoreStates.Move);
    }

    public void Reset(Vector2 position)
    {
        mainBrain.FitnessMultiplier = 1.0f;
        mainBrain.FitnessReward = 0f;
        eatBrain.FitnessMultiplier = 1.0f;
        eatBrain.FitnessReward = 0f;
        moveBrain.FitnessMultiplier = 1.0f;
        moveBrain.FitnessReward = 0f;

        counterEating = 0;
        hasEatenEnoughFood = false;
        this.position = position;
        fsm.ForceState(CarnivoreStates.Move);
    }

    public void DecideState(float[] outputs)
    {
        if (outputs[0] > 0.0f)
        {
            fsm.Transition(CarnivoreFlags.ToMove);
        }
        else if (outputs[1] > 0.0f)
        {
            fsm.Transition(CarnivoreFlags.ToEat);
        }
    }

    public void PreUpdate(float deltaTime)
    {
        Vector2 nearestFoodPosition = GetNearFoodPos();

        mainBrain.inputs = new[]
            { position.X, position.Y, nearestFoodPosition.X, nearestFoodPosition.Y, hasEatenEnoughFood ? 1 : -1, };
        moveBrain.inputs = new[] { position.X, position.Y, nearestFoodPosition.X, nearestFoodPosition.Y };
        eatBrain.inputs = new[]
            { position.X, position.Y, nearestFoodPosition.X, nearestFoodPosition.Y, hasEatenEnoughFood ? 1 : -1 };
    }

    public void Update(float deltaTime)
    {
        DecideState(mainBrain.outputs);
        fsm.Tick();
    }

    public Vector2 GetNearFoodPos()
    {
        return populationManager.GetNearHerbivore(position).position;
    }

    public Hervivoro GetNearHerbivore()
    {
        return populationManager.GetNearHerbivore(position);
    }

    public void MoveTo(Vector2 dir)
    {
        position += dir;
        if (position.X > populationManager.gridSizeX)
        {
            position.X = populationManager.gridSizeX;
        }
        else if (position.X < 0)
        {
            position.X = 0;
        }

        if (position.Y > populationManager.gridSizeY)
        {
            position.Y = populationManager.gridSizeY;
        }
        else if (position.Y < 0)
        {
            position.Y = 0;
        }
    }

    public void GiveFitnessToMain()
    {
        mainBrain.FitnessMultiplier = 1.0f;
        mainBrain.FitnessReward = 0f;
        mainBrain.FitnessReward = eatBrain.FitnessReward + moveBrain.FitnessReward;
        mainBrain.FitnessMultiplier += eatBrain.FitnessMultiplier + moveBrain.FitnessMultiplier;
    }
}

public enum CarnivoreStates
{
    Move,
    Eat,
    Escape,
    Dead,
    Corpse
}

public enum CarnivoreFlags
{
    ToMove,
    ToEat,
    ToEscape,
    ToDead,
    ToCorpse
}