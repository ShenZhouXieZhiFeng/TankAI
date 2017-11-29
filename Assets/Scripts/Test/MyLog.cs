using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class MyLog : Action {

    public SharedString text;

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

    public override void OnStart()
    {
        Debug.Log(text);
    }

    public override void OnReset()
    {
        Debug.Log("mylog is restart");
    }

    public override void OnBehaviorComplete()
    {
        Debug.Log("mylog is OnBehaviorComplete");
    }
}
