using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathTools{


    public static float Random01() {
        return Random.Range(0f, 1f);
    }

    /// <summary>
    /// 返回-1到1之间
    /// </summary>
    /// <returns></returns>
    public static float RandomClamped() {
        //return (float)(randomizer.NextDouble()*2-1);
        return Random.Range(-1f, 1f);
    }

    /// <summary>
    /// 激活函数的一种
    /// </summary>
    public static double Sigmoid(float inpute) {
        return (1.0 / (1.0 + System.Math.Exp(-inpute)));
        //if (inpute > 10) return 1.0f;
        //else if (inpute < -10) return 0.0f;
        //else return (1.0 / (1.0 + System.Math.Exp(-inpute)));
    }

    /// <summary>
    /// 判断两个向量夹角情况，顺逆时针
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static int ReturenSign(Vector3 v1,Vector3 v2) {
        if (v1.z * v2.x > v1.x * v2.z)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

}
