using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract void Enter();
    public abstract void HandleTransition();
    public abstract void Execute();
    public abstract void Exit();
}
