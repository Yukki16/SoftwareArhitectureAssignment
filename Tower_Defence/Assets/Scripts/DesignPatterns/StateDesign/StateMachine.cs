using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateMachine : MonoBehaviour
{
    private StateSO currentState;
    private StateSO previousState; // Tracks the previous state
    private Dictionary<StateSO, List<TransitionSO>> stateTransitions = new Dictionary<StateSO, List<TransitionSO>>();
    private List<TransitionSO> globalTransitions = new List<TransitionSO>(); // For transitions with multiple sources

    public void SetState(StateSO newState)
    {
        if (currentState != newState)
        {
            previousState = currentState; // Remember the previous state
            currentState = newState;
            Debug.Log($"State changed to: {currentState.name}");
        }
    }

    public void AddTransition(TransitionSO transition)
    {
        foreach (var fromState in transition.FromStates)
        {
            if (!stateTransitions.ContainsKey(fromState))
                stateTransitions[fromState] = new List<TransitionSO>();
            stateTransitions[fromState].Add(transition);
        }
    }

    public void AddGlobalTransition(TransitionSO transition)
    {
        globalTransitions.Add(transition);
    }

    public void UpdateState(string condition)
    {
        // Check specific transitions
        if (stateTransitions.TryGetValue(currentState, out var transitions))
        {
            foreach (var transition in transitions)
            {
                if (transition.condition == condition)
                {
                    SetState(transition.toState);
                    return;
                }
            }
        }

        // Check global transitions
        foreach (var transition in globalTransitions)
        {
            if (transition.condition == condition && transition.FromStates.Contains(currentState))
            {
                SetState(transition.toState);
                return;
            }
        }
    }

    public void ResumePreviousState()
    {
        if (previousState != null)
        {
            SetState(previousState);
        }
        else
        {
            Debug.LogWarning("No previous state to resume to.");
        }
    }
}
