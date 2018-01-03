using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected int Count = 0;

    public abstract void EnterSatte(StateMachine _mac);

    public abstract void UpdateState(StateMachine _mac);

    public abstract void ExitState(StateMachine _mac);
	
}
