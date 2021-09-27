using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyState : IState
{
    public void Execute(float dt) { }
    public void HandleInput() { }
    public void Enter(params object[] args) { }
    public void Exit() { }
}

public class StateMachine
{
    Dictionary<string, IState> stateDict = new Dictionary<string, IState>();
    IState current = new EmptyState();

    public IState Current { get { return current; } }
    public void Add(string id, IState state) { stateDict.Add(id, state); }
    public void Remove(string id) { stateDict.Remove(id); }
    public void Clear() { stateDict.Clear(); }

    public void Change(string id, params object[] args)
    {
        if (!stateDict.ContainsKey(id)) 
        { 
            Debug.LogError(id + " doesn't exist in stateMachine");
            return;
        }
        current.Exit();
        IState next = stateDict[id];
        next.Enter(args);
        current = next;
    }

    public void Execute(float dt)
    {
        current.Execute(dt);
    }

    public void HandleInput()
    {
        current.HandleInput();
    }

    public string GetCurrentState()
    {
        foreach(string stateName in stateDict.Keys)
        {
            IState state = stateDict[stateName];
            if(state == current)
            {
                return stateName;
            }
        }
        return "";
    }
}
