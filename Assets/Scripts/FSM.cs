using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FSM
{
    public int currentState = 0;
    private int counter = 0;
    private Dictionary<int, State> behaviour;
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviour = new Dictionary<int, State>();
        transitions = new int[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = -1;
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void SetTrasnsition(int originState, int flags, int destinationState)
    {
        transitions[originState, flags] = destinationState;
    }

    public void Transition(int flags)
    {
        if (transitions[currentState, flags] != -1)
        {
            foreach (Action behaviour in behaviour[currentState].GetTickBehaviours(behaviourOnExitParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }

            currentState = transitions[currentState, flags];

            foreach (Action behaviour in behaviour[currentState].GetTickBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }

    public void Tick()
    {
        if(behaviour.ContainsKey(currentState))
        {
            foreach (Action behaviour in behaviour[currentState].GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTrikParameters = null, 
        Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        if (!behaviour.ContainsKey(stateIndex))
        {
            State newState = new T();
            newState.OnFlag += Transition;
            behaviour.Add(stateIndex, newState);
            behaviourTickParameters.Add(stateIndex, onTrikParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

}

