using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTools{

    public static float Random01() {
        return Random.Range(0f, 1f);
    }

    public static float RandomClamped() {
        return Random.Range(-1f, 1f);    
    }

    /// <summary>
    /// 激活函数的一种
    /// </summary>
    public static double Sigmoid(float inpute) {
        if (inpute > 10) return 1.0f;
        else if (inpute < -10) return 0.0f;
        else return (1.0 / (1.0 + System.Math.Exp(-inpute)));
    }

}
