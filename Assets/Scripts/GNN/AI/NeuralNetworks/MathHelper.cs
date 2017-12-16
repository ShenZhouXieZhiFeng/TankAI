using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper{

    public static float SigmoidFunction(float xValue)
    {
        if (xValue > 10) return 1.0f;
        else if (xValue < -10) return 0.0f;
        else return (float)(1.0 / (1.0 + Math.Exp(-xValue)));
    }

    public static float TanHFunction(float xValue)
    {
        if (xValue > 10) return 1.0f;
        else if (xValue < -10) return -1.0f;
        else return (float)Math.Tanh(xValue);
    }

    public static float SoftSignFunction(float xValue)
    {
        return xValue / (1 + Math.Abs(xValue));
    }
}
