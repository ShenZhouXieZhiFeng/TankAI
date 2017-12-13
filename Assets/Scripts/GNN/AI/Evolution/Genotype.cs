using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// 基因
/// </summary>
public class Genotype : IComparable<Genotype>, IEnumerable<float>
{
    #region 属性
    private static Random randomizer = new Random();

    /// <summary>
    /// 评分,本身
    /// </summary>
    public float Evaluation
    {
        get;
        set;
    }

    /// <summary>
    /// 适应度,相对于整个种群
    /// </summary>
    public float Fitness
    {
        get;
        set;
    }

    //矩阵参数,构成神经网络
    private float[] parameters;
    public int ParameterCount
    {
        get
        {
            if (parameters == null) return 0;
            return parameters.Length;
        }
    }
    //直接访问矩阵参数
    public float this[int index] {
        get { return parameters[index]; }
        set { parameters[index] = value; }
    }



    #endregion

    #region 方法

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="pars"></param>
    public Genotype(float[] pars) {
        this.parameters = pars;
        Fitness = 0;
    }

    /// <summary>
    /// 将基因型型设置为给定范围内的随机值
    /// </summary>
    /// <param name="minVal"></param>
    /// <param name="maxVal"></param>
    public void SetRandomParameters(float minVal,float maxVal) {
        if (minVal > maxVal) throw new ArgumentException("Minimum value may not exceed maximum value.");

        float range = maxVal - minVal;
        for (int i = 0; i < parameters.Length; i++) {
            parameters[i] = (float)(randomizer.NextDouble() * range + minVal);
        }
    }

    /// <summary>
    /// 返回该参数矩阵的拷贝
    /// </summary>
    /// <returns></returns>
    public float[] GetParameterCopy() {
        float[] copy = new float[ParameterCount];
        for (int i = 0; i < ParameterCount; i++)
            copy[i] = parameters[i];

        return copy;
    }

    /// <summary>
    /// 保存到文件
    /// </summary>
    /// <param name="filePath"></param>
    public void SaveToFile(string filePath)
    {
        StringBuilder builder = new StringBuilder();
        foreach (float param in parameters)
            builder.Append(param.ToString()).Append(";");

        builder.Remove(builder.Length - 1, 1);

        File.WriteAllText(filePath, builder.ToString());
    }

    /// <summary>
    /// 从文件中读取
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static Genotype LoadFromFile(string filePath)
    {
        string data = File.ReadAllText(filePath);

        List<float> parameters = new List<float>();
        string[] paramStrings = data.Split(';');

        foreach (string parameter in paramStrings)
        {
            float parsed;
            if (!float.TryParse(parameter, out parsed)) throw new ArgumentException("The file at given file path does not contain a valid genotype serialisation.");
            parameters.Add(parsed);
        }

        return new Genotype(parameters.ToArray());
    }

    public int CompareTo(Genotype other)
    {
        return other.Fitness.CompareTo(this.Fitness);
    }

    public IEnumerator<float> GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < parameters.Length; i++)
            yield return parameters[i];
    }
    #endregion

}
