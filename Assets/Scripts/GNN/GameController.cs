using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public float PhysicFrameTime = 0.01f;
    public bool StopPhysic = false;

    private float physicSteps = 0.01f;

	void Start () {
        StartCoroutine(phySimulate());
    }
	
    IEnumerator phySimulate() {
        while (!StopPhysic) {
            Physics.Simulate(physicSteps);
            yield return new WaitForSeconds(PhysicFrameTime);
        }
        yield return 0;
    }
}
