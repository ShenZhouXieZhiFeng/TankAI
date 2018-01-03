using Radar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarWorldController : MonoBehaviour {

    public Text times;
    public Slider slider;

    public int stepsPerSecond = 1;
    public float physicStepLength = 0.02f;

    public int totalStepsPerEpoch = 1000;
    public int currentStepsInEpoch = 0;

    private int geneCount;
    public int GeneCount {
        set {
            times.text = value.ToString();
            geneCount = value;
        }
        get {
            return geneCount;
        }
    }

    private void Start()
    {
        slider.maxValue = totalStepsPerEpoch;
    }

    private void Update()
    {
        Physics.autoSimulation = false;
        if (currentStepsInEpoch > totalStepsPerEpoch) Evolve();
        for (var i = 0; i < stepsPerSecond; i++)
        {
            slider.value = currentStepsInEpoch;
            TrainingUpdate();
        }
    }

    public void Evolve()
    {
        GeneCount++;
        currentStepsInEpoch = 0;
        RadarEvolutionManager.Instance.Restart();
    }

    public void TrainingUpdate()
    {
        currentStepsInEpoch++;
        Physics.Simulate(physicStepLength);
        RadarTankManager.Instance.TrainTanks();
    }
}
