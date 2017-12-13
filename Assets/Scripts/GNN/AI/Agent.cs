using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// tank大脑
/// </summary>
public class Agent : IComparable<Agent>
{

    #region 属性

    /// <summary>
    /// 这个大脑代表的基因型
    /// </summary>
    public Genotype Genotype {
        get;
        private set;
    }

    /// <summary>
    /// 这个大脑的神经网络,由基因型构成
    /// </summary>
    public NeuralNetwork FNN
    {
        get;
        private set;
    }

    private bool isAlive = false;
    public bool IsAlive {
        get { return isAlive; }
        set {
            if (isAlive != value) {
                isAlive = value;
                if (!isAlive && AgentDied != null)
                    AgentDied(this);
            }
        }
    }

    /// <summary>
    /// 实体死亡时回调
    /// </summary>
    public event Action<Agent> AgentDied;

    #endregion

    #region 方法

    public Agent(Genotype genotype, NeuralLayer.ActivationFunction defauActivation,params uint[] topogy) {
        IsAlive = false;

        this.Genotype = genotype;

        FNN = new NeuralNetwork(topogy);
        // 指定该神经网络每一层的激活函数
        foreach (NeuralLayer layer in FNN.Layers)
            layer.NeuronActivationFunction = defauActivation;

        if(FNN.WeightCount != genotype.ParameterCount)
            throw new ArgumentException("The given genotype's parameter count must match the neural network topology's weight count.");

        //从基因型构造模糊神经网络
        IEnumerator<float> parameters = genotype.GetEnumerator();
        foreach (NeuralLayer layer in FNN.Layers) //Loop over all layers
        {
            for (int i = 0; i < layer.Weights.GetLength(0); i++) //Loop over all nodes of current layer
            {
                for (int j = 0; j < layer.Weights.GetLength(1); j++) //Loop over all nodes of next layer
                {
                    layer.Weights[i, j] = parameters.Current;
                    parameters.MoveNext();
                }
            }
        }

    }

    public int CompareTo(Agent other)
    {
        return Genotype.CompareTo(other.Genotype);
    }

    public void Reset() {
        Genotype.Evaluation = 0;
        Genotype.Fitness = 0;
        IsAlive = true;
    }

    public void Kill() {
        IsAlive = false;
    }

    #endregion

}
