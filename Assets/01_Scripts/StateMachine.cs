using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseState
{
    private StateMachine stateMachine;

    public void SetStateMachine(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    protected void ChangeState(string stateName)
    {
        stateMachine.ChangeState(stateName);
    }

    protected void ChangeState<T>(T stateName) where T : Enum
    {
        ChangeState(stateName.ToString());
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void Transition() { }
}
public class StateMachine : MonoBehaviour
{
    private Dictionary<string, BaseState> statesDic = new();
    private BaseState currentState;

    private void Start()
    {
        currentState.Enter();
    }

    private void Update()
    {
        currentState.Update();
        currentState.Transition();
    }

    public void LateUpdate()
    {
        currentState.LateUpdate();
    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void InitState(string stateName)
    {
        if (!statesDic.ContainsKey(stateName))
        {
            Debug.LogError($"StateMachine::InitState: State '{stateName}' not found!");
            return;
        }
        currentState = statesDic[stateName];
    }

    public void AddState(string stateName, BaseState state)
    {
        if (statesDic.ContainsKey(stateName))
        {
            Debug.LogError($"StateMachine::AddState: State '{stateName}' already exists!");
            return;
        }

        state.SetStateMachine(this);
        statesDic.Add(stateName, state);
    }

    public void ChangeState(string stateName)
    {
        if (!statesDic.ContainsKey(stateName))
        {
            Debug.LogError($"StateMachine::ChangeState: State '{stateName}' not found!");
            return;
        }

        currentState.Exit();
        currentState = statesDic[stateName];
        currentState.Enter();
    }

    public void InitState<T>(T stateType) where T : Enum
    {
        InitState(stateType.ToString());
    }

    public void AddState<T>(T stateType, BaseState state) where T : Enum
    {
        AddState(stateType.ToString(), state);
    }

    public void ChangeState<T>(T stateType)
    {
        ChangeState(stateType.ToString());
    }
}
