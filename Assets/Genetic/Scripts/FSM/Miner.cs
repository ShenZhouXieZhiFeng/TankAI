using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour {

    StateMachine _stateMac;

	// Use this for initialization
	void Start () {
        _stateMac = new StateMachine(new FirstState());
        InvokeRepeating("UpdateSelf", 0, 1);
    }

    void UpdateSelf()
    {
        _stateMac.UpdateState();
    }
}
