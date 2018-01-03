using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private State currentState;
    private State preState;

    public StateMachine(State _initState)
    {
        ChangeState(_initState);
    }

    public void UpdateState()
    {
        if (currentState == null)
            return;
        currentState.UpdateState(this);
    }

    public void ChangeState(State _targetState)
    {
        if (currentState != null) {
            preState = currentState;
            currentState.ExitState(this);
        }
        currentState = _targetState;
        currentState.EnterSatte(this);
    }

    public void RevertState()
    {
        if (preState != null)
            ChangeState(preState);
    }
}

public class FirstState : State
{
    public override void EnterSatte(StateMachine _mac)
    {
        Debug.Log("Enter " + this.GetType());
    }

    public override void UpdateState(StateMachine _mac)
    {
        Debug.Log("Update " + this.GetType());
        Count++;
        if (Count > 3)
        {
            Count = 0;
            _mac.ChangeState(new SecondState());
        }
    }

    public override void ExitState(StateMachine _mac)
    {
        Debug.Log("Exit " + this.GetType());
    }
}

public class SecondState : State
{
    public override void EnterSatte(StateMachine _mac)
    {
        Debug.Log("Enter " + this.GetType());
    }

    public override void UpdateState(StateMachine _mac)
    {
        Debug.Log("Update " + this.GetType());
        Count++;
        if (Count > 3)
        {
            Count = 0;
            _mac.ChangeState(new ThirdState());
        }
    }

    public override void ExitState(StateMachine _mac)
    {
        Debug.Log("Exit " + this.GetType());
    }
}

public class ThirdState : State
{
    public override void EnterSatte(StateMachine _mac)
    {
        Debug.Log("Enter " + this.GetType());
    }

    public override void UpdateState(StateMachine _mac)
    {
        Debug.Log("Update " + this.GetType());
        Count++;
        if (Count > 3)
        {
            Count = 0;
            _mac.ChangeState(new FirstState());
        }
    }

    public override void ExitState(StateMachine _mac)
    {
        Debug.Log("Exit " + this.GetType());
    }
}


