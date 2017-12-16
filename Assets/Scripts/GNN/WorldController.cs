using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    public int stepsPerSecond = 1;
    public float physicStepLength = 0.02f;

    public int totalStepsPerEpoch = 1000;

    public int currentStepsInEpoch = 0;

    private void Update()
    {
        Physics.autoSimulation = false;
        if (currentStepsInEpoch > totalStepsPerEpoch) Evolve();
        for (var i = 0; i < stepsPerSecond; i++)
        {
            TrainingUpdate();
        }
    }

    public void Evolve()
    {
        currentStepsInEpoch = 0;
        EvolutionManager.Instance.Restart();
    }

    public void TrainingUpdate()
    {
        currentStepsInEpoch++;
        Physics.Simulate(physicStepLength);
        TankManager.Instance.TrainTanks();
    }
}
